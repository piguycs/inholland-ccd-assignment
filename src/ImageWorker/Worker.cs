using Azure.Storage.Queues;
using System.Text.Json;
using ImageWorker.Models;

namespace ImageWorker;

public class Worker(
        IConfiguration configuration,
        ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {

        var connectionString = configuration["Storage:connectionString"]
            ?? throw new InvalidOperationException("Storage:ConnectionString is not configured");

        var queue = new QueueClient(connectionString, "image-processing");

        await queue.CreateIfNotExistsAsync(cancellationToken: ct);

        while (!ct.IsCancellationRequested)
        {
            var response = await queue.ReceiveMessageAsync(cancellationToken: ct);
            var message = response.Value;

            if (message is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
                continue;
            }

            var request = JsonSerializer.Deserialize<ImageRequest>(
                    message.Body.ToString(),
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (request is null)
            {
                logger.LogWarning("Received an invalid image request");
                await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
                continue;
            }

            logger.LogInformation(
                    "Received image job for {StationName} in generation {GenerationId}",
                    request.WeatherStation.StationName,
                    request.GenerationId);

            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
        }
    }
}
