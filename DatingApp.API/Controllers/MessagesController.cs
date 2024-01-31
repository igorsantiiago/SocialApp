using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers;

public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername();
        if (username == createMessageDTO.RecipientUsername.ToLower())
            return BadRequest("Não é possível enviar mensagem para si mesmo.");

        var sender = await _unitOfWork.UserRepository.GetUserByUsername(username);
        if (sender == null)
            return NotFound();
        var recipient = await _unitOfWork.UserRepository.GetUserByUsername(createMessageDTO.RecipientUsername);
        if (recipient == null)
            return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName!,
            RecipientUsername = recipient.UserName!,
            Content = createMessageDTO.Content
        };

        _unitOfWork.MessageRepository.AddMessage(message);
        if (await _unitOfWork.Complete())
            return Ok(_mapper.Map<MessageDTO>(message));

        return BadRequest("Falha ao enviar a mensagem");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _unitOfWork.MessageRepository.GetMessage(id);
        if (message == null)
            return NotFound();

        if (message.SenderUsername != username && message.RecipientUsername != username)
            return Unauthorized();

        if (message.SenderUsername == username)
            message.SenderDeleted = true;
        if (message.RecipientUsername == username)
            message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
            _unitOfWork.MessageRepository.DeleteMessage(message);

        if (await _unitOfWork.Complete())
            return Ok();

        return BadRequest("Problema ao deletar a mensagem");
    }
}
