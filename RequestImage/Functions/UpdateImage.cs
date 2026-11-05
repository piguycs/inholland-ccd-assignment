using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RequestImage.Dto;

namespace RequestImage.Functions;

public class UpdateImage(ILogger<UpdateImage> logger, HttpClient httpClient)
{
    private const string ImageApi = "https://picsum.photos/1200/800";

    [Function(nameof(UpdateImage))]
    public async Task Run([QueueTrigger("image-update-queue")] WeatherQueueMessage message)
    {
        logger.LogInformation("Fetching image for job {JobId} in {Region}", message.JobId, message.Region);

        using var response = await httpClient.GetAsync(ImageApi);
        response.EnsureSuccessStatusCode();

        using var imageStream = await response.Content.ReadAsStreamAsync();

        logger.LogInformation("Downloaded image for job {JobId}: {Size} bytes", message.JobId, imageStream.Length);
    }
}