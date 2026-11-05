namespace RequestImage.Dto;

public record WeatherQueueMessage
{
    public required Guid ImageJobId { get; init; }
    public required Guid JobId { get; init; }
    public required string Region { get; init; }
    public required float Temperature { get; init; }
    public required string Description { get; init; }
}