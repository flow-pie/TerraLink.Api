using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{

    /// <summary>
    /// Configuration for automatically generated regulatory and management
    /// reports.
    /// </summary>
    [Table("report_schedules")]
    public class ReportSchedule
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("report_name")]
        public string ReportName { get; set; } = null!;

        [Required]
        [Column("frequency")]
        public ReportFrequency Frequency { get; set; }

        [Required]
        [Column("next_run")]
        public DateTime NextRun { get; set; }

        [Column("enabled")]
        public bool Enabled { get; set; } = true;
    }
}