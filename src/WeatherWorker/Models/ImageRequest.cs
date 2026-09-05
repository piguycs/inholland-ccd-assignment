namespace WeatherWorker.Models;

public sealed record ImageRequest(
        Guid GenerationId,
        WeatherStation WeatherStation);
