using JobPortalApplication.Data;
using JobPortalApplication.Interfaces;
using JobPortalApplication.Models;


namespace JobPortalApplication.Repository
{
    public class PostedJobsRepository : IPostedJobsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostedJobsRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Job>> GetAllUserJobs()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userJobs = _context.Jobs.Where(r => r.AppUser.Id == curUser);
            return userJobs.ToList();
        }
    }
}
