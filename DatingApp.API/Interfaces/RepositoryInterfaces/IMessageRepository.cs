﻿using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(int id);
    Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserUsername, string recipientUsername);

    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection(string connectionId);
}
