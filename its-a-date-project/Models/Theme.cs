using System.ComponentModel.DataAnnotations;

namespace its_a_date_project.Models
{
    public class Theme
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string BgStart { get; set; } = "#ffeaf4";

        [Required, MaxLength(20)]
        public string BgEnd { get; set; } = "#ffd2e6";

        [Required, MaxLength(20)]
        public string CardBg { get; set; } = "#fffbfd";

        [Required, MaxLength(20)]
        public string Ink { get; set; } = "#5c1f3d";

        [Required, MaxLength(20)]
        public string InkSoft { get; set; } = "#93486e";

        [Required, MaxLength(20)]
        public string Accent { get; set; } = "#ff5c9e";

        [Required, MaxLength(20)]
        public string AccentDeep { get; set; } = "#e0227a";

        [Required, MaxLength(20)]
        public string AccentSoft { get; set; } = "#ffd3e7";

        [Required, MaxLength(20)]
        public string Gold { get; set; } = "#ffb84d";

        [Required, MaxLength(20)]
        public string Border { get; set; } = "#ffcfe4";

        [Required, MaxLength(40)]
        public string ShadowRgba { get; set; } = "rgba(224,34,122,0.18)";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<Invite> Invites { get; set; } = new();
    }
}
