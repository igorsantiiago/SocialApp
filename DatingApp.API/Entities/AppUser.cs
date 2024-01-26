using DatingApp.API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Entities;

public class AppUser : IdentityUser<int>
{
    public DateOnly BirthDate { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<Photo> Photos { get; set; } = new();


    public List<UserLike> LikedByUsers { get; set; } = null!;
    public List<UserLike> LikedUsers { get; set; } = null!;

    public List<Message> MessagesSent { get; set; } = null!;
    public List<Message> MessagesReceived { get; set; } = null!;

    public ICollection<AppUserRole> UserRoles { get; set; } = null!;
}
