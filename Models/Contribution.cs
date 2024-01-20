using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundStream.Models
{
    public class Contribution
    {
        [Key] // Marks Id as the primary key
        public int ContributionId { get; set; }

        [Required] // The UserId is required
        public int UserId { get; set; }

        [Required] // The ProjectId is required
        public int ProjectId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Ensures the amount is stored with two decimal places
        [Range(0.01, double.MaxValue, ErrorMessage = "Contribution amount must be positive.")] // Ensures the amount is positive
        public decimal Amount { get; set; }

        [DataType(DataType.DateTime)] // Tracks when the contribution was made
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        // Virtual keyword enables EF's lazy loading
        public virtual User Contributor { get; set; } // The user who made the contribution
        public virtual Project Project { get; set; } // The project to which the contribution was made
    }
}