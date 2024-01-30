using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.API.SignalR;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _presenceHub = presenceHub;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var user = Context.User!.GetUsername();
        var otherUser = httpContext!.Request.Query["user"];

        var groupName = GetGroupName(user, otherUser!);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _messageRepository.GetMessageThread(user, otherUser!);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        var username = Context.User!.GetUsername();
        if (username == createMessageDTO.RecipientUsername.ToLower())
            throw new HubException("Não é possível enviar mensagem a si próprio.");

        var sender = await _userRepository.GetUserByUsername(username) 
            ?? throw new HubException("Usuário não encontrado.");
        var recipient = await _userRepository.GetUserByUsername(createMessageDTO.RecipientUsername) 
            ?? throw new HubException("Destinatário não encontrado.");

        var message = new Message
        {
            Sender = sender,
            SenderUsername = sender.UserName!,
            Recipient = recipient,
            RecipientUsername = recipient.UserName!,
            Content = createMessageDTO.Content
        };

        var groupName = GetGroupName(sender.UserName!, recipient.UserName!);
        var group = await _messageRepository.GetMessageGroup(groupName);
        if(group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName!);
            if(connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new
                {
                    username = sender.UserName,
                    knownAs = sender.KnownAs
                });
            }
        }

        _messageRepository.AddMessage(message);

        if(await _messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }

    private string GetGroupName(string caller, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
        return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        if(group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        var connection = new Connection(Context.ConnectionId, Context.User!.GetUsername());
        group.Connections.Add(connection);

        if(await _messageRepository.SaveAllAsync())
            return group;

        throw new HubException("Falha ao adicionar ao grupo.");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

        _messageRepository.RemoveConnection(connection!);

        if(await _messageRepository.SaveAllAsync()) 
            return group;

        throw new HubException("Falha ao remover do grupo");
    }
}
