using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RequestImage.Functions;

public class GetImages(ILogger<GetImages> logger, BlobServiceClient blobServiceClient)
{
    private readonly BlobContainerClient _container = blobServiceClient.GetBlobContainerClient("images");
    
    [Function(nameof(GetImages))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "images/{imageJobId:guid}")] HttpRequest req,
                    Guid imageJobId)
    {
        var prefix = $"{imageJobId}/";

        var urls = new List<string>();
        
        await foreach (var blob in _container.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = _container.GetBlobClient(blob.Name);

            var sasUrl = blobClient.GenerateSasUri(
                BlobSasPermissions.Read,
                DateTimeOffset.UtcNow.AddHours(1)
            );
            
            urls.Add(sasUrl.ToString());
        }

        if (urls.Count == 0)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(urls);
    }
}