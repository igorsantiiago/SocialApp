using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    public AdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("user-with-roles")]
    public async Task<ActionResult> GetUsesWithRoles()
    {
        var users = await _userManager.Users.OrderBy(x => x.UserName).Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles))
            return BadRequest("É necessário a inserção de pelo menos um cargo(role).");

        var selectedRoles = roles.Split(",").ToArray();
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded)
            return BadRequest("Falha ao adicionar os cargos(roles).");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded)
            return BadRequest("Falha ao remover os cargos(roles).");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "RequireModeratorRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Somente Admin ou Moderadores conseguem acessar essa página");
    }
}
