using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed.")]
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }
        public string Name => $"{FirstName} {LastName}";
    }

    public class UserRegisterDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed.")]
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Name => $"{FirstName} {LastName}";
    }
}
