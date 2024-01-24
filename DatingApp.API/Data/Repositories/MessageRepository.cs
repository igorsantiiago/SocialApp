using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async void AddMessage(Message message)
        => await _context.Messages.AddAsync(message);

    public void DeleteMessage(Message message)
        => _context.Messages.Remove(message);

    public async Task<Message?> GetMessage(int id)
        => await _context.Messages.FindAsync(id);

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false && x.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await _context.Messages
            .Include(s => s.Sender).ThenInclude(p => p.Photos)
            .Include(r => r.Recipient).ThenInclude(p => p.Photos)
            .Where(
                x => x.RecipientUsername == currentUsername && x.RecipientDeleted == false &&
                x.SenderUsername == recipientUsername ||
                x.RecipientUsername == recipientUsername && x.SenderDeleted == false &&
                x.SenderUsername == currentUsername
            ).OrderBy(m => m.MessageSent).ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public async Task<bool> SaveAllAsync()
        => await _context.SaveChangesAsync() > 0;
}
