using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RequestImage.Dto;

namespace RequestImage.Functions;

public class ProcessImageRequest(ILogger<ProcessImageRequest> logger)
{
    [Function(nameof(ProcessImageRequest))]
    public void Run([QueueTrigger("image-queue")] ImageQueueMessage message)
    {
        logger.LogInformation("C# Queue Trigger: job {JobId} with {Status}", message.JobId, message.Status);
    }
}