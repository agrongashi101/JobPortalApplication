using JobPortalApplication.Data;
using JobPortalApplication.Interfaces;
using JobPortalApplication.Models;
using JobPortalApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortalApplication.Services;
using Application = JobPortalApplication.Models.Application;
using Microsoft.AspNetCore.Authorization;

namespace JobPortalApplication.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        

        public JobController(IJobRepository jobRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _jobRepository = jobRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Job> jobs = await _jobRepository.GetAll();
            return View(jobs);
        }


        public async Task<IActionResult> Detail(int id)
        {
            Job job = await _jobRepository.GetByIdAsync(id);
            return View(job);
        }

        [Authorize(Roles = "Admin,Employer")]
        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createJobViewModel = new CreateJobViewModel { AppUserId = curUserId };
            return View(createJobViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(CreateJobViewModel jobVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(jobVM.Image);

                var job = new Job
                {
                    Position = jobVM.Position,
                    JobTypeCategory = jobVM.JobTypeCategory,
                    Description = jobVM.Description,
                    Salary = jobVM.Salary,
                    CompanyName = jobVM.CompanyName,
                    Image = result.Url.ToString(),
                    AppUserId = jobVM.AppUserId,
                    Address = new Address
                    {
                        Street = jobVM.Address.Street,
                        City = jobVM.Address.City
                    },
                    PostedTime = DateTime.UtcNow 
                };

                _jobRepository.Add(job);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(jobVM);
        }

        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(int id) 
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                return View("Error");

     
            var currentUserId = User.GetUserId();
            if (job.AppUserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Unauthorized("You are not authorized to edit this job.");
            }

            var jobVM = new EditJobViewModel
            {
                Position = job.Position,
                JobTypeCategory = job.JobTypeCategory,
                Description = job.Description,
                Salary = job.Salary,
                CompanyName = job.CompanyName,
                AddressId = job.AddressId,
                Address = job.Address,
                URL = job.Image 
            };

            return View(jobVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Edit(int id, EditJobViewModel jobVM) 
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit job. Please check your input.");
                return View(jobVM);
            }

            var userJob = await _jobRepository.GetByIdAsyncNoTracking(id);

            if (userJob == null)
            {
                ModelState.AddModelError("", "Job not found.");
                return View(jobVM);
            }

     
            var currentUserId = User.GetUserId();
            if (userJob.AppUserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Unauthorized("You are not authorized to edit this job.");
            }

            string newImageUrl = userJob.Image; 

   
            if (jobVM.Image != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(userJob.Image))
                    {
                        await _photoService.DeletePhotoAsync(userJob.Image);
                    }

                    var photoResult = await _photoService.AddPhotoAsync(jobVM.Image);
                    newImageUrl = photoResult.Url.ToString(); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Failed to upload new photo.");
                    return View(jobVM);
                }
            }

       
            var updatedJob = new Job
            {
                Id = id,
                Position = jobVM.Position,
                JobTypeCategory = jobVM.JobTypeCategory,
                Description = jobVM.Description,
                Salary = jobVM.Salary,
                CompanyName = jobVM.CompanyName,
                Image = newImageUrl,
                AddressId = jobVM.AddressId,
                Address = jobVM.Address,
                AppUserId = userJob.AppUserId 
            };

            try
            {
                _jobRepository.Update(updatedJob);
                return RedirectToAction("Index", "PostedJobs");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update the job. Please try again.");
                return View(jobVM);
            }
        }

        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Delete(int id) 
        {
            var jobDetails = await _jobRepository.GetByIdAsync(id);
            if (jobDetails == null) return View("Error");
            return View(jobDetails); 
        }

        [HttpPost, ActionName("Delete")] 
        [Authorize(Roles = "Admin,Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id) 
        {
            var jobDetails = await _jobRepository.GetByIdAsync(id);
            if (jobDetails == null) return View("Error");

            _jobRepository.Delete(jobDetails);
            return RedirectToAction("Index"); 
        }

        [HttpGet]
        public async Task<IActionResult> SearchJobsByPosition(string searchTerm)
        {
            var allJobs = await _jobRepository.GetAll();

            var jobs = string.IsNullOrEmpty(searchTerm)
                ? allJobs
                : allJobs.Where(j => j.Position.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            var results = jobs.Select(j => new
            {
                j.Id,
                j.Position,
                Description = j.Description.Length > 100
                    ? j.Description.Substring(0, 100) + "..."
                    : j.Description,
                j.Image,
                j.PostedTime, 
            }).ToList();

            return Json(results);
        }


        [HttpPost]
        public async Task<IActionResult> Apply(int jobId, IFormFile cv, string applicantName, string applicantEmail)
        { 
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantEmail == applicantEmail);

            if (existingApplication != null)
            {
                TempData["ApplicationError"] = "You have already applied for this job!";
                return RedirectToAction("Detail", new { id = jobId });
            }

            var application = new Application
            {
                JobId = jobId,
                ApplicantName = applicantName,
                ApplicantEmail = applicantEmail,
                CVPath = $"/uploads/{cv.FileName}" 
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["ApplicationSuccess"] = "Your application has been submitted successfully!";
            return RedirectToAction("Detail", new { id = jobId });
        }

        [Authorize(Roles = "Admin,Employer")]
        public IActionResult ViewApplications(int jobId)
        {
            var applications = _context.Applications
                                        .Where(a => a.JobId == jobId)
                                        .Select(a => new ApplicationViewModel
                                        {
                                            Id = a.Id,
                                            ApplicantName = a.ApplicantName,
                                            ApplicantEmail = a.ApplicantEmail,
                                            CVPath = a.CVPath
                                        }).ToList();

            
            ViewBag.JobId = jobId; 
            return View(applications);
        }
    }
}