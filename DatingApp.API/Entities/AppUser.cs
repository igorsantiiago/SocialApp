﻿namespace DatingApp.API.Entities;

public class AppUser
{
    public AppUser()
    {

    }
    public AppUser(string username)
    {
        UserName = username;
    }

    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
}
