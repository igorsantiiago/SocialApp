﻿using System.ComponentModel.DataAnnotations;

namespace DatingApp.API;

public class RegisterAccountDTO
{
    [Required(ErrorMessage = "Username required.")]
    [MinLength(3, ErrorMessage = "Username minimum length is 3 characters.")]
    [MaxLength(20, ErrorMessage = "Username maximum length is 20 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password minimum length is 8 characters.")]
    [MaxLength(50, ErrorMessage = "Password maximum length is 50 characters.")]
    public string Password { get; set; } = string.Empty;
}
