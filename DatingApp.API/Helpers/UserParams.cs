namespace DatingApp.API.Helpers;

public class UserParams
{
    public const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string CurrentUsername = string.Empty;
    public string? Gender { get; set; } = string.Empty;

    public int MinimumAge { get; set; } = 18;
    public int MaximumAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActivity";
}
