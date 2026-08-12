using System.ComponentModel.DataAnnotations;

namespace its_a_date_project.Models.ViewModels
{
    public class ThemeFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; } = "";

        [Required] public string BgStart { get; set; } = "#ffeaf4";
        [Required] public string BgEnd { get; set; } = "#ffd2e6";
        [Required] public string CardBg { get; set; } = "#fffbfd";
        [Required] public string Ink { get; set; } = "#5c1f3d";
        [Required] public string InkSoft { get; set; } = "#93486e";
        [Required] public string Accent { get; set; } = "#ff5c9e";
        [Required] public string AccentDeep { get; set; } = "#e0227a";
        [Required] public string AccentSoft { get; set; } = "#ffd3e7";
        [Required] public string Gold { get; set; } = "#ffb84d";
        [Required] public string Border { get; set; } = "#ffcfe4";

        [Required, MaxLength(40)]
        public string ShadowRgba { get; set; } = "rgba(224,34,122,0.18)";

        public string? Error { get; set; }
    }
}
