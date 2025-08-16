using Microsoft.AspNetCore.Identity;

namespace MyDotNet9Api.Services;

public class UserServices: IUserServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager;

    public UserServices(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<string> ObtainUserId()
    {
        var email = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == "email")!.Value;
        var user = await _userManager.FindByEmailAsync(email);
        return user!.Id;
    }
}