using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RequestImage.Dto;

namespace RequestImage.Functions;

public class ProcessImageRequest(ILogger<ProcessImageRequest> logger, HttpClient httpClient)
{
    private const string BuienRadarApi = "https://data.buienradar.nl/2.0/feed/json";
    private static readonly string[] RegionWhitelist = ["Amsterdam", "Rotterdam", "Utrecht"];
    
    [Function(nameof(ProcessImageRequest))]
    [QueueOutput("image-update-queue")]
    public async Task<IEnumerable<WeatherQueueMessage>> Run([QueueTrigger("image-queue")] ImageQueueMessage message)
    {
        logger.LogInformation("C# Queue Trigger: job {JobId} with {Status}", message.JobId, message.Status);
        
        WeatherInfo? weatherInfo;
        
        {
            using var response = await httpClient.GetAsync(BuienRadarApi);
            response.EnsureSuccessStatusCode();
            weatherInfo = await response.Content.ReadFromJsonAsync<WeatherInfo>();
        }

        if (weatherInfo is null)
        {
            throw new Exception("Weather Info is null");
        }

        var regions = weatherInfo.Actual.StationMeasurements
            .Where(x => RegionWhitelist.Contains(x.Regio))
            .Select(x => new WeatherQueueMessage
            {
                ImageJobId = message.JobId,
                JobId = Guid.NewGuid(),
                Region = x.Regio,
                Temperature = x.Temperature,
                Description = x.WeatherDescription
            });

        return regions;
    }
}