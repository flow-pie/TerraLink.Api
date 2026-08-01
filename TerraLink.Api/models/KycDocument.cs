using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{

    /// <summary>
    /// Stores individual KYC document captures per client, supporting both
    /// registration channels.
    /// </summary>

    public class KycDocs
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
        [Column("doc_type")]
        public KycDocType DocType { get; set; }


        [Required]
        [MaxLength(255)]
        [Column("file_url")]
        public string FileUrl { get; set; } = null!;


        [Column("verified")]
        public bool Verified { get; set; } = false;


        [Column("verified_by")]
        public long? VerifiedBy { get; set; }

        [ForeignKey(nameof(VerifiedBy))]
        public User? VerifiedByUser { get; set; }


        [Column("verified_at")]
        public DateTime? VerifiedAt { get; set; }


        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}