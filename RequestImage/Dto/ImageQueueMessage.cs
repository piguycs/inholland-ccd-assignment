namespace RequestImage.Dto;

public record ImageQueueMessage
{
    public required Guid JobId { get; init; }
}