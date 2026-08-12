namespace its_a_date_project.Models
{
    public class AdminSetting
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}
