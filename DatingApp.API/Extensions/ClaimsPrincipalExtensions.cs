using System.Security.Claims;

namespace DatingApp.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Name)!.Value;

    public static string GetUserId(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
}
