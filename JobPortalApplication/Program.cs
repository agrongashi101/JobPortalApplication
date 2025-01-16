using JobPortalApplication.Data;
using JobPortalApplication.Helpers;
using JobPortalApplication.Interfaces;
using JobPortalApplication.Models;
using JobPortalApplication.Repository;
using JobPortalApplication.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IPostedJobsRepository, PostedJobsRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); ;
});
builder.Services.AddIdentity<AppUser, IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession(); 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie();


builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 10 * 1024 * 1024; // 10 MB limit for file size
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB limit for multipart data
});


// Enable serving static files
builder.Services.AddControllersWithViews();


var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "jobdatadata")
{
    await JobData.SeedUsersAndRolesAsync(app);
    //allow to seed the data
    //JobData.JobDataData(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
