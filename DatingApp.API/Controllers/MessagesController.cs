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
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername();
        if (username == createMessageDTO.RecipientUsername.ToLower())
            return BadRequest("Não é possível enviar mensagem para si mesmo.");

        var sender = await _userRepository.GetUserByUsername(username);
        if (sender == null)
            return NotFound();
        var recipient = await _userRepository.GetUserByUsername(createMessageDTO.RecipientUsername);
        if (recipient == null)
            return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync())
            return Ok(_mapper.Map<MessageDTO>(message));

        return BadRequest("Falha ao enviar a mensagem");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _messageRepository.GetMessage(id);
        if (message == null)
            return NotFound();

        if (message.SenderUsername != username && message.RecipientUsername != username)
            return Unauthorized();

        if (message.SenderUsername == username)
            message.SenderDeleted = true;
        if (message.RecipientUsername == username)
            message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
            _messageRepository.DeleteMessage(message);

        if (await _messageRepository.SaveAllAsync())
            return Ok();

        return BadRequest("Problema ao deletar a mensagem");
    }
}
