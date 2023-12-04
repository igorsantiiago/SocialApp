namespace DatingApp.API;

public class UserResponseDTO
{
    public UserResponseDTO(string username, string token)
    {
        Username = username;
        Token = token;
    }
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
