using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NatuurlikBase.Models;

namespace NatuurlikBase.Factory
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager
           , RoleManager<IdentityRole> roleManager
           , IOptions<IdentityOptions> optionsAccesor) : base(userManager, roleManager, optionsAccesor)
        {
        }
    }
}
