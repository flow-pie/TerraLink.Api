using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs;

public record UpdateUserDto(
    [StringLength(50)] string? EmployeeId,
    [StringLength(100), Required] string FullName,
    [EmailAddress, StringLength(100)] string? Email,
    [Phone] string Phone,
    [StringLength(100), Required] string Password,
    [StringLength(50), Required] string Role,
    [StringLength(20), Required] string Status
);