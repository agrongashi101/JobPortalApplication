namespace JobPortalApplication.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string CVPath { get; set; }
        public Job Job { get; set; }
    }
}
