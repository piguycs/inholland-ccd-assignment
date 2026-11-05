using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RequestImage.Dto;

namespace RequestImage.Functions;

public class RequestImage(ILogger<RequestImage> logger)
{
    [Function(nameof(RequestImage))]
    public ImageOutput Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "images")] HttpRequest req)
    {
        var jobId = Guid.NewGuid();
        const JobStatus status = JobStatus.Pending;
        
        var response = new ImageQueueMessage
        {
            JobId = jobId,
            Status = status,
        };
        
        logger.LogInformation("Queuing {JobId} for image request with status {Status}", jobId, status);
        
        var location = $"/api/images/{jobId}";

        return new ImageOutput
        {
            QueueMessage = response,
            HttpResponse = new AcceptedResult(location, response),
        };
    }
}