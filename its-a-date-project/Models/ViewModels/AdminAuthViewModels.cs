using System.ComponentModel.DataAnnotations;

namespace its_a_date_project.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; } = "";
        public string? Error { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; } = "";

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = "";

        [Required]
        public string ConfirmPassword { get; set; } = "";

        public string? Error { get; set; }
        public string? Success { get; set; }
    }
}
