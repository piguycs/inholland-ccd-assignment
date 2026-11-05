using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RequestImage.Dto;

namespace RequestImage.Functions;

public class RequestImage(ILogger<RequestImage> logger, BlobServiceClient blobServiceClient)
{
    private readonly BlobContainerClient _container = blobServiceClient.GetBlobContainerClient("images");
    [Function(nameof(RequestImage))]
    public async Task<ImageOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "images")] HttpRequest req)
    {
        var jobId = Guid.NewGuid();
        
        var response = new ImageQueueMessage
        {
            JobId = jobId,
        };
        
        // Persist acceptance before the queue output can start processing the job.
        await _container.GetBlobClient($"_jobs/{jobId}")
            .UploadAsync(BinaryData.FromString("{}"), cancellationToken: req.HttpContext.RequestAborted);

        logger.LogInformation("Queuing {JobId} for image request", jobId);
        
        var location = $"/api/images/{jobId}";

        return new ImageOutput
        {
            QueueMessage = response,
            HttpResponse = new AcceptedResult(location, response),
        };
    }
}
