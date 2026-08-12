namespace its_a_date_project.Models.ViewModels
{
    public class InviteViewModel
    {
        public Invite Invite { get; set; } = null!;
        public Theme Theme { get; set; } = null!;
        public Dictionary<string, string> Text { get; set; } = new();
        public string Lang { get; set; } = "en";
        public bool IsRtl => Lang == "ar";
    }
}
