using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NDHuy_W345_S2.Models
{
    public class ApplicationUser : IdentityUser 
    {
        [Required]
        public string FullName {  get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
    }
}
