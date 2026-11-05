using System.Text.Json.Serialization;

namespace RequestImage.Dto;

public record WeatherInfo
{
    
    [JsonPropertyName("actual")]
    public required WeatherInfoActual Actual { get; init; }
}