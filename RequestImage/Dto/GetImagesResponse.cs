namespace RequestImage.Dto;

public record GetImagesResponse
{
    public required Guid JobId { get; init; }
    public required IReadOnlyList<ImageResponse> Images { get; init; }
}

public record ImageResponse
{
    public required string Url { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}
