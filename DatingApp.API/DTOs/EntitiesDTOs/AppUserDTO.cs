namespace DatingApp.API.DTOs.EntitiesDTO;

public class AppUserDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public int Age { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivity { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<PhotoDTO> Photos { get; set; } = new();
}
