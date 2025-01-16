using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using JobPortalApplication.Data.Enum;

namespace JobPortalApplication.Models
{
    public class Job
    { 


        [Key]
        public int Id { get; set; }
        public string Position { get; set; }
        public JobTypeCategory JobTypeCategory { get; set; }
        public string Description { get; set; }
        public string Salary { get; set; }
        public string CompanyName { get; set; }
        public string Image { get; set; }
        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public Address Address { get; set; }
        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public DateTime PostedTime { get; set; }
    }
}
