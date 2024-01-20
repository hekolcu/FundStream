using System;
using System.ComponentModel.DataAnnotations;

namespace FundStream.Models
{
    public class User
    {
        [Key] // This data annotation defines the property below as the primary key
        public int UserId { get; set; } // Auto-incremented primary key

        [Required] // This property is required
        [StringLength(50, MinimumLength = 2)] // String length constraints
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress] // Validates the property to contain a valid email address
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] // Marks the property as a password field
        [MinLength(8)] // Minimum length for the password
        public string Password { get; set; }

        // You can add additional properties for timestamps, navigation properties, etc.

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default value
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties for related entities can also be included, for example:
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Contribution> Contributions { get; set; }
    }
}