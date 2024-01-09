namespace DatingApp.API.Entities;

public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsProfile { get; set; }
    public string PublicId { get; set; } = string.Empty;

    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
}
