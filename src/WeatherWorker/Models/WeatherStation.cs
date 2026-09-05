namespace WeatherWorker.Models;

public sealed record WeatherStation(
    int StationId,
    string StationName,
    decimal? Temperature,
    string? WeatherDescription);