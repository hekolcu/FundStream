using System;
using System.ComponentModel.DataAnnotations;

namespace FundStream.Models
{
    public class User
    {
        public User()
        {
            Projects = new HashSet<Project>();
            Contributions = new HashSet<Contribution>();
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty; // Initialize with default value

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Contribution> Contributions { get; set; }
    }

}