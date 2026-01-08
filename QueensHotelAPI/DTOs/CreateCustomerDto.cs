using System;
using System.ComponentModel.DataAnnotations;

namespace QueensHotelAPI.DTOs
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        public string LName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;
        
        // Password is now optional for customer creation
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; } = string.Empty;
        
        //public int Status { get; set; }
        public string? PassportId { get; set; }
        public string? NIC { get; set; }
        
        [Required(ErrorMessage = "Country of residence is required")]
        public int CountryOfResidence_id { get; set; }
        
        public string? DOB { get; set; }
        public string? Gender { get; set; }
    }
}

