using System.Net.Http.Json;
using Azure.Storage.Queues;
using System.Text.Json;
using WeatherWorker.Models;

namespace WeatherWorker;

public class Worker(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<Worker> logger) : BackgroundService
{
    private const string BuienradarEndpoint = "https://data.buienradar.nl/2.0/feed/json";

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var connectionString = configuration["Storage:ConnectionString"]
            ?? throw new InvalidOperationException("Storage:ConnectionString is not configured");

        var queue = new QueueClient(connectionString, "generation-requests");

        while (!ct.IsCancellationRequested)
        {
            var response = await queue.ReceiveMessageAsync(cancellationToken: ct);
            var message = response.Value;

            if (message is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
                continue;
            }

            var request = JsonSerializer.Deserialize<GenerationRequest>(
                    message.Body.ToString(),
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (request is null)
            {
                logger.LogWarning("Received an invalid generation request");
                continue;
            }

            logger.LogInformation(
                    "Processing generation {GenerationId}, requested at {RequestedAt}",
                    request.GenerationId,
                    request.RequestedAt);

            var client = httpClientFactory.CreateClient();

            var feed = await client.GetFromJsonAsync<WeatherFeed>(
                    BuienradarEndpoint,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            var stations = feed?.Actual?.StationMeasurements;

            if (stations is null)
            {
                logger.LogError("Buienradar returned invalid data");
                continue;
            }

            logger.LogInformation(
                    "Fetched {StationCount} weather stations for generation id {GenerationId}",
                    stations.Count,
                    request.GenerationId);

            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
        }
    }
}
