using Azure.Storage.Queues;
using System.Text.Json;

namespace WeatherWorker;

public class Worker(
        IConfiguration configuration,
        ILogger<Worker> logger) : BackgroundService
{
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

            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
        }
    }
}
