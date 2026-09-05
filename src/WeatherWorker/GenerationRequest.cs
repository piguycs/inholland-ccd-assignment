namespace WeatherWorker;

public record GenerationRequest(Guid GenerationId, DateTimeOffset RequestedAt);
