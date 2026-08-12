namespace its_a_date_project.Models
{
    public class DateSubmission
    {
        public int Id { get; set; }

        public int InviteId { get; set; }
        public Invite? Invite { get; set; }

        public DateTime ChosenDateUtc { get; set; }
        public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
        public bool EmailSent { get; set; }
    }
}
