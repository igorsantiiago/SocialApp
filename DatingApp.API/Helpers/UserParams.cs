namespace DatingApp.API.Helpers;

public class UserParams : PaginationParams
{
    public string CurrentUsername = string.Empty;
    public string? Gender { get; set; } = string.Empty;

    public int MinimumAge { get; set; } = 18;
    public int MaximumAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActivity";
}
