using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandiCraft.Domain.Identity;
using Microsoft.AspNetCore.Http;
using static HandiCraft.Domain.Identity.ApplicationUser;


namespace HandiCraft.Application.DTOs.User
{
    public class RegisterDto
    {
        
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = """
            Password must contain:
            - 8+ characters
            - 1 uppercase letter
            - 1 lowercase letter
            - 1 digit
            - 1 special character
            """)]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        
        public UserType UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ProfilePicture { get; set; } 
    }
}
