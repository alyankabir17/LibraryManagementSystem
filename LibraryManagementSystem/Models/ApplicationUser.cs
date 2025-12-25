using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        public string Role { get; set; } = "User";
    }
}
