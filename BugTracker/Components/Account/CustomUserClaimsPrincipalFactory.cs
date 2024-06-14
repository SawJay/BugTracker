using BugTracker.Client;
using BugTracker.Data;
using BugTracker.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BugTracker.Components.Account
{
    public class CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                                                        IOptions<IdentityOptions> options)
                                            : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
    {

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            string profilePictureUrl = user.ProfilePictureId.HasValue ? $"/api/uploads/{user.ProfilePictureId}" : UploadHelper.DefaultProfilePicture;

            List<Claim> customClaims =
            [
                new Claim(nameof(UserInfo.FirstName), user.FirstName!),
                new Claim(nameof(UserInfo.LastName), user.LastName!),
                new Claim(nameof(UserInfo.ProfilePictureUrl), profilePictureUrl!),
                new Claim("CompanyId", user.CompanyId.ToString()),
            ];

            identity.AddClaims(customClaims);

            return identity;
        }

    }
}
