namespace WeatherWorker.Models;

public sealed record GenerationRequest(Guid GenerationId, DateTimeOffset RequestedAt);
