using JobPortalApplication.Data.Enum;
using JobPortalApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace JobPortalApplication.Data
{
    public class JobData
    {
        public static void JobDataData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                if (!context.Jobs.Any())
                {
                    context.Jobs.AddRange(new List<Job>()
                    {
                        new Job()
                        {
                            Position = "Web Development",
                            JobTypeCategory = JobTypeCategory.FullTime,
                            Description = "We are seeking a skilled Web Developer to join our dynamic team. The ideal candidate will have a strong foundation in front-end and/or back-end development, a passion for creating user-friendly websites, and the ability to solve technical challenges.",
                            Salary = "800 Euro",
                            CompanyName = "LinkPlus",
                            Image = "https://i0.wp.com/garonpower.com/wp-content/uploads/2019/01/computer-programming.jpeg?fit=1500%2C1000&ssl=1",
                            Address = new Address()
                            {
                                Street = "B Street",
                                City = "Prishtine",
                            }
                        },
                       new Job()
                        {
                            Position = "Frontend Developer",
                            JobTypeCategory = JobTypeCategory.PartTime,
                            Description = "We are seeking a skilled Frontend Developer to join our dynamic team. The ideal candidate will have a strong foundation in front-end and/or back-end development, a passion for creating user-friendly websites, and the ability to solve technical challenges.",
                            Salary = "450 Euro",
                            CompanyName = "LinkPlus",
                            Image = "https://i0.wp.com/garonpower.com/wp-content/uploads/2019/01/computer-programming.jpeg?fit=1500%2C1000&ssl=1",
                            Address = new Address()
                            {
                                Street = "Agim Ramadani",
                                City = "Prishtine",
                            }
                        },
                       new Job()
                        {
                            Position = "Web Development",
                            JobTypeCategory = JobTypeCategory.Internship,
                            Description = "We are seeking a skilled Web Developer to join our dynamic team. The ideal candidate will have a strong foundation in front-end and/or back-end development, a passion for creating user-friendly websites, and the ability to solve technical challenges.",
                            Salary = "350 Euro",
                            CompanyName = "StarLabs",
                            Image = "https://i0.wp.com/garonpower.com/wp-content/uploads/2019/01/computer-programming.jpeg?fit=1500%2C1000&ssl=1",
                            Address = new Address()
                            {
                                Street = "B Street",
                                City = "Prishtine",
                            }
                        },
                       new Job()
                        {
                            Position = "Backend Developer",
                            JobTypeCategory = JobTypeCategory.FullTime,
                            Description = "We are seeking a skilled Backend Developer to join our dynamic team. The ideal candidate will have a strong foundation in front-end and/or back-end development, a passion for creating user-friendly websites, and the ability to solve technical challenges.",
                            Salary = "1500 Euro",
                            CompanyName = "LinkPLus",
                            Image = "https://i0.wp.com/garonpower.com/wp-content/uploads/2019/01/computer-programming.jpeg?fit=1500%2C1000&ssl=1",
                            Address = new Address()
                            {
                                Street = "B Street",
                                City = "Prishtine",
                            }
                        }
                    });
                    context.SaveChanges();
                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Employer))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Employer));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "agrondeveloper@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        UserName = "agron",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        Address = new Address()
                        {
                            Street = "Street 1",
                            City = "Lipjan"
                        }
                    };
                    await userManager.CreateAsync(newAdminUser, "Agron@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string employerUserEmail = "employer@gmail.com";

                var employerUser = await userManager.FindByEmailAsync(employerUserEmail);
                if (employerUser == null)
                {
                    var newEmployerUser = new AppUser()
                    {
                        UserName = "employer-user",
                        Email = employerUserEmail,
                        EmailConfirmed = true,
                        Address = new Address()
                        {
                            Street = "Street 2",
                            City = "New York"
                        }
                    };
                    await userManager.CreateAsync(newEmployerUser, "Employer@1234?");
                    await userManager.AddToRoleAsync(newEmployerUser, UserRoles.Employer); 
                }

                string appUserEmail = "user@gmail.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                        Address = new Address()
                        {
                            Street = "Street 3",
                            City = "Lipjan",
                        }
                    };
                    await userManager.CreateAsync(newAppUser, "User@12345?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }
    }
}
