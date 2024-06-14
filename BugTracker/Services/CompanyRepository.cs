using BugTracker.Data;
using BugTracker.Model;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IServiceProvider svcProvider) : ICompanyRepository
    {
        public async Task AddUserToRoleAsync(string userId, string roleName, string adminId)
        {
            // nobody can change their own roles, so don't let them
            if (userId == adminId) return;

            using IServiceScope scope = svcProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // get the user trying to change someone's role
            ApplicationUser? admin = await userManager.FindByIdAsync(adminId);

            // verify that they're an admin
            if (admin is not null && await userManager.IsInRoleAsync(admin, nameof(Roles.Admin)))
            {
                // get the user that they're trying to change
                ApplicationUser? user = await userManager.FindByIdAsync(userId);

                // verify that they belong to the same company
                if (user is not null && user.CompanyId == admin.CompanyId)
                {
                    IList<string> currentRoles = await userManager.GetRolesAsync(user);
                    string? currentRole = currentRoles.FirstOrDefault(r => r != nameof(Roles.DemoUser));

                    // if the user is already in the role, don't do anything
                    if (string.Equals(currentRole, roleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                    // users should only have one role at a time, so we'll remove their current role if (!string.IsNullOrEmpty(currentRole))
                    if (!string.IsNullOrEmpty(currentRole))
                    {
                        await userManager.RemoveFromRoleAsync(user, currentRole);
                    }
                    // add the new role
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Company? company = await context.Companies.Include(c => c.Members).Include(c => c.Projects).Include(c => c.Invites).FirstOrDefaultAsync(c => c.Id == id);

            return company;
        }

        public async Task<string> GetUserRoleAsync(string userId, int companyId)
        {
            // create a UserManager just for this method, similar to creating a DbContext
            using IServiceScope scope = svcProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // find the user
            ApplicationUser? user = await userManager.FindByIdAsync(userId);

            string role = "Unknown";

            // make sure the user belongs to the company
            if (user?.CompanyId == companyId)
            {
                // get their roles
                IList<string> roles = await userManager.GetRolesAsync(user);
                // some users have their assigned role AND a DemoUser role, but we don't want to show that,
                // so look up their first role that isn't DemoUser
                role = roles.FirstOrDefault(r => r != nameof(Roles.DemoUser), role);

            }

            // return whatever role we found
            return role;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            using IServiceScope scope = svcProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IList<ApplicationUser> users = await userManager.GetUsersInRoleAsync(roleName);
            IEnumerable<ApplicationUser> companyUsers = users.Where(u => u.CompanyId == companyId);

            return companyUsers;
        }

        public async Task UpdateCompanyAsync(Company company, string adminId)
        {
            using IServiceScope scope = svcProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // get the user trying to update company
            ApplicationUser? admin = await userManager.FindByIdAsync(adminId);

            if (admin is not null && await userManager.IsInRoleAsync(admin, nameof(Roles.Admin)))
            {
                using ApplicationDbContext context = contextFactory.CreateDbContext();

                bool shouldUpdate = await context.Companies.AnyAsync(c => c.Id == company.Id);

                if (shouldUpdate)
                {
                    FileUpload? oldImage = null;

                    if (company.Image is not null)
                    {
                        // save the new image
                        context.Files.Add(company.Image);

                        // check for an old image
                        oldImage = await context.Files.FirstOrDefaultAsync(i => i.Id == company.ImageId);

                        // fix the foreign key
                        company.ImageId = company.Image.Id;
                    }

                    context.Companies.Update(company);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
