using System.ComponentModel.DataAnnotations;

namespace NueroDrive.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }

        public string? PhoneNumber { get; set; }
    }

    //public class FaceRecognitionViewModel
    //{
    //    [Required]
    //    public string CarId { get; set; }

    //    [Required]
    //    public string ImageBase64 { get; set; }
    //}

    public class AddDriverViewModel
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public string DriverName { get; set; }

        [Required]
        public string FaceImageBase64 { get; set; }
    }
} 