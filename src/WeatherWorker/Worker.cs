using Azure.Storage.Queues;

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

            if (message is null) {
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
                continue;
            }

            logger.LogInformation("Received generation request: {Message}", message.Body.ToString());

            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
        }
    }
}
