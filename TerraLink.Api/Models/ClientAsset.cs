using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{

    /// <summary>
    /// Individual collateral / asset items recorded for a client, used as a
    /// scoring input during appraisal.
    /// </summary>
    public class ClientAsset
    {
        public long Id { get; set; }

        [Required]
        public long ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Required]
        public AssetType AssetType { get; set; }

        [MaxLength(150)]
        public string? Description { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}