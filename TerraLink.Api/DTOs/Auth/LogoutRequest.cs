using System.ComponentModel.DataAnnotations;

public class LogoutRequest
{
    [Required]
    public string RefreshToken { get; set; }
        = string.Empty;
}