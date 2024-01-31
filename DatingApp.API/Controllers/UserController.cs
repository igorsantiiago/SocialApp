using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;
    public UserController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet("search/list")]
    public async Task<ActionResult<PagedList<AppUserDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
        if (gender == null)
            return BadRequest();

        userParams.CurrentUsername = User.GetUsername();

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "masculino" ? "feminino" : "masculino";
        }

        var users = await _unitOfWork.UserRepository.GetAppUsersDtoAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    [HttpGet("search/id/{id}")]
    public async Task<ActionResult<AppUserDTO>> GetUserById(int id)
    {
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (user is null)
            return Ok("Usuário não encontrado.");

        return _mapper.Map<AppUserDTO>(user);
    }

    [HttpGet("search/username/{username}")]
    public async Task<ActionResult<AppUserDTO>> GetUserByUsername(string username)
    {
        var currentUsername = User.GetUsername();
        return await _unitOfWork.UserRepository.GetAppUserDtoByUsername(username, isCurrentUser: currentUsername == username);
        // var user = await _unitOfWork.UserRepository.GetAppUserDtoByUsername(username);
        // return user!;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(AppUserUpdateDTO appUserUpdateDto)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsername(User.GetUsername());

        if (user == null)
            return NotFound();

        _mapper.Map(appUserUpdateDto, user);

        if (await _unitOfWork.Complete())
            return NoContent();

        return BadRequest("Falha ao atualizar usuário.");
    }

    [HttpPost("photo/upload")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsername(User.GetUsername());
        if (user == null)
            return NotFound();

        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error != null)
            return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.Url.AbsoluteUri,
            PublicId = result.PublicId
        };
        if (user.Photos.Count == 0)
            photo.IsProfile = true;

        user.Photos.Add(photo);

        if (await _unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetUserByUsername),
                new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
        }
        return BadRequest("Falha ao adicionar foto.");
    }

    [HttpPut("photo/update/profile-picture/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsername(User.GetUsername());
        if (user == null)
            return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null)
            return NotFound();

        if (photo.IsProfile)
            return BadRequest("Esta imagem já a foto de perfil selecionada.");

        var currentProfilePicture = user.Photos.FirstOrDefault(x => x.IsProfile);
        if (currentProfilePicture != null)
            currentProfilePicture.IsProfile = false;

        photo.IsProfile = true;

        if (await _unitOfWork.Complete())
            return NoContent();

        return BadRequest("Erro ao selecionar foto de perfil.");
    }

    [HttpDelete("photo/delete/{photoId}")]
    public async Task<ActionResult> DeletePicture(int photoId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsername(User.GetUsername());
        if (user == null)
            return NotFound();

        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null)
            return NotFound();

        if (photo.IsProfile)
            return BadRequest("Não é possível excluir a foto de perfil.");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _unitOfWork.Complete())
            return Ok();

        return BadRequest("Falha ao deletar a imagem.");
    }
}
