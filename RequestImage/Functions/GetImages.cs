using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using RequestImage.Dto;

namespace RequestImage.Functions;

public class GetImages(BlobServiceClient blobServiceClient)
{
    private readonly BlobContainerClient _container = blobServiceClient.GetBlobContainerClient("images");
    
    [Function(nameof(GetImages))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "images/{imageJobId:guid}")] HttpRequest req,
                    Guid imageJobId)
    {
        var prefix = $"{imageJobId}/";

        var cancellationToken = req.HttpContext.RequestAborted;
        var knownJob = (await _container.GetBlobClient($"_jobs/{imageJobId}")
            .ExistsAsync(cancellationToken)).Value;
        var images = new List<ImageResponse>();
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);
        
        await foreach (var blob in _container.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            var blobClient = _container.GetBlobClient(blob.Name);

            var sasUrl = blobClient.GenerateSasUri(
                BlobSasPermissions.Read,
                expiresAt
            );
            
            images.Add(new ImageResponse
            {
                Url = sasUrl.ToString(),
                ExpiresAt = expiresAt
            });
        }

        // Existing images also identify jobs created before acceptance tracking was added.
        if (!knownJob && images.Count == 0)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(new GetImagesResponse
        {
            JobId = imageJobId,
            Images = images
        });
    }
}