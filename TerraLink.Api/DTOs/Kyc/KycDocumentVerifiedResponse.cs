using System.Text.Json.Serialization;
using TerraLink.Api.Models;

public record KycDocumentVerifiedResponse(
    long Id,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    KycDocType DocType,

    bool Verified,

    long? VerifiedBy,

    DateTime? VerifiedAt
);