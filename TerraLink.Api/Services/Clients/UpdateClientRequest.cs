using System.ComponentModel.DataAnnotations;

public class UpdateClientRequest
{
    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(120)]
    public string? FullName { get; set; }

    [StringLength(8)]
    public string? NationalId { get; set; }
}