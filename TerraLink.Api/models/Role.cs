using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TerraLink.Api.Models;

[Table("Roles")]
public class Role
{
    public long Id {get; set;}

    [MaxLength(50)]
    public required string Name {get; set;}
    
    [MaxLength(255)]
    public string? Description {get; set;}
}