using EmberFrameworksService.Models.UserInformation;

namespace EmberFrameworksService.Managers;

public class UserManager
{
    public User castSQLToUser(Dictionary<int, object> userResult)
    {
        User user = new();
        try
        {
            user.Id = (string)userResult[0];
            user.Biography = (string)userResult[1];
            user.GeoLocation = (string)userResult[2];
            user.Permissions = (string)userResult[3];
            user.Badges = (string)userResult[4];
            user.Titles = (string)userResult[5];
        }
        catch (Exception e)
        {
            throw e;
        }
        return user;
    }
}