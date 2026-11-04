using System.Text.Json.Serialization;

namespace RequestImage.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobStatus
{
    Pending,
    Complete,
}