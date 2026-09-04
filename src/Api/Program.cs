using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = builder.Configuration["Storage:ConnectionString"]
    ?? throw new InvalidOperationException("Storage:ConnectionString is not configured");

var queue = new QueueClient(connectionString, "generation-requests");
var images = new BlobContainerClient(connectionString, "generated-images");

app.MapPost("/generations", async (CancellationToken ct) => 
{
    await queue.CreateIfNotExistsAsync(cancellationToken: ct);

    var generationId = Guid.NewGuid();
    var message = new { generationId, requestedAt = DateTimeOffset.UtcNow };

    await queue.SendMessageAsync(JsonSerializer.Serialize(message), cancellationToken: ct);

    return Results.Accepted($"/generations/{generationId}", new { generationId });
});

app.MapGet("/generations/{generationId:guid}/images", async (Guid generationId, CancellationToken ct) =>
{
    await images.CreateIfNotExistsAsync(cancellationToken: ct);

    var imageUrls = new List<string>();

    var iterator = images.GetBlobsAsync(
            traits: BlobTraits.None,
            states: BlobStates.None,
            prefix: $"{generationId}/",
            cancellationToken: ct);

    await foreach (var blob in iterator) {
        imageUrls.Add(images.GetBlobClient(blob.Name).Uri.AbsolutePath);
    }

    return Results.Ok(new { generationId, images = imageUrls });
});


app.Run();
