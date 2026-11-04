namespace RequestImage.Dto;

public record ImageResp
{
    public required Guid JobId { get; init; }
    public required JobStatus Status { get; init; }
}