namespace TerraLink.Api.DTOs
{
    //for GET /api/users list view.
    public record UserListItemDto
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? EmployeeNo { get; set; }
        public string RoleName { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}