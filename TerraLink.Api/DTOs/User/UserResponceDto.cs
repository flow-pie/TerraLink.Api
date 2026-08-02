namespace TerraLink.Api.DTOs
{

    // // Returned by GET /api/users/me and GET /api/users/{id}. 
    //SHOULD NOT contain all the information such as password hash, MFA secret, etc.
    public record UserResponseDto
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? EmployeeNo { get; set; }
        public string RoleName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public bool MfaEnabled { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}