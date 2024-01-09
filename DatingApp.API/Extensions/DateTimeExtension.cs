namespace DatingApp.API.Extensions;

public static class DateTimeExtension
{
    public static int CalculateAge(this DateOnly birthDate)
    {
        var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = todayDate.Year - birthDate.Year;

        if (birthDate > todayDate.AddYears(-age))
            age--;

        return age;
    }
}
