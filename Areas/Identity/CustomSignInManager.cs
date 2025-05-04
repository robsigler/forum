using System.Linq;

namespace Forum.Areas.Identity;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

public class CustomSignInManager<TUser> : SignInManager<TUser> where TUser : IdentityUser
{
    private readonly UserManager<TUser> _userManager;

    public CustomSignInManager(UserManager<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _userManager = userManager;
    }

    public override async Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null)
    {
        var isNewUser = !await _userManager.HasPasswordAsync(user) &&
                        (await GetExternalAuthenticationSchemesAsync()).Any();

        await base.SignInAsync(user, isPersistent, authenticationMethod);

        if (isNewUser && user.GetType().GetProperty("JoinDate") != null)
        {
            user.GetType().GetProperty("JoinDate")?.SetValue(user, DateTime.UtcNow);
            await _userManager.UpdateAsync(user);
        }
    }
}