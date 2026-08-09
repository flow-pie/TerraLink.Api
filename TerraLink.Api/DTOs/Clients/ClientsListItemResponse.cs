namespace TerraLink.Api.DTOs.Clients
{
    //for GET /api/users list view.
    public record ClientsListItemResponse
    (
        long Id,
        string? Username,
        string? Email,
        string? EmployeeNo,
        string RoleName,
        string Status,
        DateTime? LastLogin
    );
}