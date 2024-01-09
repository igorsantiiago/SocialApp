namespace DatingApp.API.DTOs.EntitiesDTO;

public class PhotoDTO
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsProfile { get; set; }
}
