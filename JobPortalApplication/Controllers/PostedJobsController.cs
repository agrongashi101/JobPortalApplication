using JobPortalApplication.Interfaces;
using JobPortalApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApplication.Controllers
{
    public class PostedJobsController : Controller
    {
        private readonly IPostedJobsRepository _postedJobsRepository;
       
        public PostedJobsController(IPostedJobsRepository postedJobsRepository) 
        {
            _postedJobsRepository = postedJobsRepository;
        }
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Index()
        {
            var userJobs = await _postedJobsRepository.GetAllUserJobs();
            var PostedJobsViewModel = new PostedJobsViewModel()
            {
                Jobs = userJobs
            };
            return View(PostedJobsViewModel);
        }
    }
}
