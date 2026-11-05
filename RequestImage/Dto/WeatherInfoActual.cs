using System.Text.Json.Serialization;

namespace RequestImage.Dto;

public record WeatherInfoActual
{
    [JsonPropertyName("stationmeasurements")]
    public required IEnumerable<StationMeasurements> StationMeasurements { get; init; }
}