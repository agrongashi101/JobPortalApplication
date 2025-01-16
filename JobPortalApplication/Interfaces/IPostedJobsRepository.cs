using JobPortalApplication.Models;

namespace JobPortalApplication.Interfaces
{
    public interface IPostedJobsRepository
    {
        Task<List<Job>> GetAllUserJobs();
    }
}
