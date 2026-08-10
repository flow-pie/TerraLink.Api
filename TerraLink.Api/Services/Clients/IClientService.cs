using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.Clients;

namespace TerraLink.Api.Services.Clients;

public interface IClientService
{
    Task<ClientRegistrationResult> RegisterAsync(
        RegisterClientRequest request,
        long? authenticatedUserId,
        CancellationToken cancellationToken
    );

    Task<PagedResponse<ClientsListItemResponse>> 
    GetAllClientsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );

    Task<ClientProfileResponse?> GetClientByIdAsync(
        long userId,
        CancellationToken cancellationToken
    );
}