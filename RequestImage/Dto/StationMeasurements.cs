using System.Text.Json.Serialization;

namespace RequestImage.Dto;

public record StationMeasurements
{
    [JsonPropertyName("regio")]
    public required string Regio { get; init; }
    
    [JsonPropertyName("weatherdescription")]
    public required string WeatherDescription { get; set; }
    
    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }
}