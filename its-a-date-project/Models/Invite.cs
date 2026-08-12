using System.ComponentModel.DataAnnotations;

namespace its_a_date_project.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Slug { get; set; } = string.Empty;

        public int ThemeId { get; set; }
        public Theme? Theme { get; set; }

        [Required, MaxLength(320)]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required, MaxLength(2)]
        public string DefaultLanguage { get; set; } = "en";

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<InviteText> Texts { get; set; } = new();
        public List<DateSubmission> Submissions { get; set; } = new();
    }
}
