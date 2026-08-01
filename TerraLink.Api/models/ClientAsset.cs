using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{

    /// <summary>
    /// Individual collateral / asset items recorded for a client, used as a
    /// scoring input during appraisal.
    /// </summary>
    [Table("client_assets")]
    public class ClientAsset
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Required]
        [Column("asset_type")]
        public AssetType AssetType { get; set; }

        [MaxLength(150)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Column("recorded_at")]
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}