using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace RequestImage.Dto;

public record ImageOutput
{
    [QueueOutput("image-queue")]
    public required ImageResp QueueMessage { get; init; }

    [HttpResult]
    public required IActionResult HttpResponse { get; init; }
}