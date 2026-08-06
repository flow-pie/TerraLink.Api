using TerraLink.Api.DTOs.Clients;

namespace TerraLink.Api.Services.Clients;

public interface IClientService
{
    Task<ClientRegistrationResult> RegisterAsync(
        RegisterClientRequest request,
        long? authenticatedUserId,
        CancellationToken cancellationToken
    );
}