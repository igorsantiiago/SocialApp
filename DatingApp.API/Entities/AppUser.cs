namespace DatingApp.API.Entities;

public class AppUser
{
    protected AppUser()
    {

    }
    public AppUser(string username)
    {
        UserName = username;
    }

    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}
