using System.Text.Json.Serialization;

namespace JobBoard.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Status
{
    Submitted,
    UnderReview,
    Accepted,
    Rejected
}
