using System.Text.Json.Serialization;
using TerraLink.Api.Models;

public record KycDocumentResponse(
    long Id,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    KycDocType DocType,

    string? FileUrl,

    bool Verified,

    DateTime? VerifiedAt
);