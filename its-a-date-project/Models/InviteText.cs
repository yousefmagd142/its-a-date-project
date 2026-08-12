using System.ComponentModel.DataAnnotations;

namespace its_a_date_project.Models
{
    public static class TextKeys
    {
        public static readonly string[] All =
        {
            "Eyebrow", "Headline", "Subhead", "YesButton", "NoButton",
            "DodgeLine1", "DodgeLine2", "DodgeLine3", "DodgeLine4", "DodgeLine5", "DodgeLine6",
            "PlanEyebrow", "PlanHeadline", "PlanSubhead", "DateLabel", "TimeLabel", "SubmitButton",
            "ThanksEyebrow", "ThanksHeadline", "ThanksMessageTemplate", "LoveLine"
        };
    }

    public class InviteText
    {
        public int Id { get; set; }

        public int InviteId { get; set; }
        public Invite? Invite { get; set; }

        [Required, MaxLength(2)]
        public string Language { get; set; } = "en";

        [Required, MaxLength(40)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
