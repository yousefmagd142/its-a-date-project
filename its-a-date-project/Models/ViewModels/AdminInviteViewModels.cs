using System.ComponentModel.DataAnnotations;

namespace its_a_date_project.Models.ViewModels
{
    public class InviteFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(120), RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Lowercase letters, numbers, and hyphens only.")]
        public string Slug { get; set; } = "";

        [Required]
        public int ThemeId { get; set; }

        [Required, EmailAddress]
        public string RecipientEmail { get; set; } = "";

        [Required]
        public string DefaultLanguage { get; set; } = "en";

        public bool IsActive { get; set; } = true;

        public List<Theme> AvailableThemes { get; set; } = new();

        public string? Error { get; set; }
    }

    public class TextFieldViewModel
    {
        public string Key { get; set; } = "";
        public string English { get; set; } = "";
        public string Arabic { get; set; } = "";
    }

    public class InviteTextEditViewModel
    {
        public int InviteId { get; set; }
        public string Slug { get; set; } = "";
        public List<TextFieldViewModel> Fields { get; set; } = new();
        public string? Success { get; set; }
    }
}
