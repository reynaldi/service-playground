using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Playground.Auth.Domain
{
    public class AuthUser : IdentityUser
    {
        [MaxLength(150), Required]
        public string FirstName { get; set; }
        [MaxLength(150), Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
