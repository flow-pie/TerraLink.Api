using System.ComponentModel.DataAnnotations;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.Clients;

public class RegisterClientRequest
{
    [MaxLength(120)]
    public required string FullName { get; set; }

    [StringLength(8, MinimumLength = 8, ErrorMessage = "National ID must contain exactly 8 characters")]
    public required string NationalId { get; set; }

    [MaxLength(10)]
    public required string Phone { get; set; }

    public required DateTime DateOfBirth { get; set; }

    public required Gender Gender { get; set; }

    [MaxLength(500)]
    public required string Address { get; set; }

    [EmailAddress]
    [MaxLength(120)]
    public string? Email { get; set; }

    [MinLength(8)]
    [MaxLength(128)]
    public string? Password { get; set; }

    //KYC documents
    public required IFormFile NationalIdFront { get; set; }
    public required IFormFile NationalIdBack { get; set; }
    public required IFormFile PassportPhoto { get; set; }
}