using System.Security.Claims;

namespace DatingApp.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim!;
    }
}
