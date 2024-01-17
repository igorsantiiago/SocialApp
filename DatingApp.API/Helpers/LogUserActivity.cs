using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity!.IsAuthenticated)
            return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var repository = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await repository.GetUserByIdAsync(int.Parse(userId));


        user!.LastActivity = DateTime.UtcNow;
        await repository.SaveAllAsync();

    }
}
