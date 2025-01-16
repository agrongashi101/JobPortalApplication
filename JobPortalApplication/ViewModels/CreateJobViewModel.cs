using JobPortalApplication.Data.Enum;
using JobPortalApplication.Models;

namespace JobPortalApplication.ViewModels
{
    public class CreateJobViewModel
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public JobTypeCategory JobTypeCategory { get; set; }
        public string Description { get; set; }
        public string Salary { get; set; }
        public string CompanyName { get; set; }
        public Address Address { get; set; }
        public IFormFile Image { get; set; }
        public string AppUserId { get; set; }
    }
}
