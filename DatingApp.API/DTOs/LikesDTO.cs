﻿namespace DatingApp.API.DTOs;

public class LikesDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
