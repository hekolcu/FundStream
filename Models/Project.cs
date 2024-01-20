using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundStream.Models
{
    public class Project
    {
        public Project()
        {
            // Initialize the Contributions collection to ensure it's never null
            Contributions = new HashSet<Contribution>();
        }

        [Key] // Defines this property as the primary key in the database
        public int ProjectId { get; set; }

        [Required] // This property is required
        [StringLength(255)] // Sets a maximum length for the project title
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(20)] // Ensures the description has at least 20 characters
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GoalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountRaised { get; set; } = 0; // Default value

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Foreign key for the User who created the project
        public int UserId { get; set; }
        
        // Navigation property for the User who created the project
        public virtual User? Creator { get; set; }

        // Navigation property for contributions made to the project
        public virtual ICollection<Contribution> Contributions { get; set; }
    }
}