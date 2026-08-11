using System.Text.Json.Serialization;
using TerraLink.Api.Models;

public record RejectClientResponse(
    long ClientId,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    VerificationStatus VerificationStatus,
    
    string Message
);