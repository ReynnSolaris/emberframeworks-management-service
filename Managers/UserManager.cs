using EmberFrameworksService.Models.UserInformation;

namespace EmberFrameworksService.Managers;

public class UserManager
{
    public User castSQLToUser(Dictionary<string, object> userResult)
    {
        User user = new();
        try
        {
            user.Id = (string)userResult["Id"];
            user.Biography = (string)userResult["Biography"];
            user.GeoLocation = (string)userResult["GeoLocation"];
            user.Permissions = (string)userResult["Permissions"];
            user.Badges = (string)userResult["Badges"];
            user.Titles = (string)userResult["Titles"];
        }
        catch (Exception e)
        {
            throw e;
        }
        return user;
    }
}