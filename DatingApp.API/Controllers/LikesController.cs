using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public LikesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);
        if (sourceUser.UserName == username)
            return BadRequest("Você não pode curtir o seu próprio perfil.");

        var likedUser = await _unitOfWork.UserRepository.GetUserByUsername(username);
        if (likedUser == null)
            return NotFound();

        var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
        if (userLike != null)
            return BadRequest("Você já curtiu essa pessoa.");

        var targetUserId = likedUser.Id;

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = targetUserId
        };

        sourceUser.LikedUsers.Add(userLike);
        if (await _unitOfWork.Complete())
            return Ok();

        return BadRequest("Falha ao curtir usuário.");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikesDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

        Response.AddPaginationHeader(new(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
        return Ok(users);
    }
}
