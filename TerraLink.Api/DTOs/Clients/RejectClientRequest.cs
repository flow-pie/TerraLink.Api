using System.ComponentModel.DataAnnotations;

public class RejectClientRequest
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}