using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(20, ErrorMessage = "This field must contain between 3 and 20 characters")]
        [MinLength(3, ErrorMessage = "This field must contain between 3 and 20 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(20, ErrorMessage = "This field must contain between 3 and 20 characters")]
        [MinLength(3, ErrorMessage = "This field must contain between 3 and 20 characters")]
        public string Password { get; set; }
        
        
        public string Role { get; set; }
    }
}