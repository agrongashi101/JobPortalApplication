using JobPortalApplication.Models;
using static System.Net.Mime.MediaTypeNames;
using Application = JobPortalApplication.Models.Application;

namespace JobPortalApplication.Interfaces
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAll();
        Task<Job> GetByIdAsync(int id);
        Task<Job> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Job>> GetJobByCity(string city);
        bool Add(Job job);
        bool Update(Job job); 
        bool Delete(Job job);
        bool Save();
    
        Task<IEnumerable<Application>> GetApplicationsByJobId(int jobId);
    }
}