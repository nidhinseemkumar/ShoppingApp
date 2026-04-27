using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public partial class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed in First Name.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed in Last Name.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must have a valid domain.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MinLength(10, ErrorMessage = "Address must be at least 10 characters long.")]
        public string? Address { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public string? Role { get; set; } = "Customer";

        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
