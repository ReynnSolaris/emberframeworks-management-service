using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace EmberFrameworksService.Managers.Firebase;

public class FirebaseManager
{
    private FirebaseAuth _firebaseAuth;
    private FirebaseApp _firebaseApp;
    public FirebaseManager()
    {
        if (FirebaseApp.DefaultInstance != null)
        {
            _firebaseApp = FirebaseApp.DefaultInstance;
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }
        else
        {
            _firebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Directory.GetCurrentDirectory() +
                                                       "/Assets/Credentials/emberframeworks-fc84d-firebase-adminsdk-7ru3s-ae830a4c12.json")
            });
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }
    }

    public async Task<bool> VerifyToken(string userId, string userToken)
    {
        try
        {
            var verifyIdTokenAsync = await _firebaseAuth.VerifyIdTokenAsync(userToken);
            return verifyIdTokenAsync.Uid == userId;
        }
        catch (FirebaseAuthException e)
        {
            return false;
        }
    }
    
    public async Task<string> GetUserToken(string uid)
    {
        return await _firebaseAuth.CreateCustomTokenAsync(uid);
    }
    
    public async Task<UserRecord> getUser(string UID)
    {
        UserRecord? user = await _firebaseAuth.GetUserAsync(UID);
        if (user != null)
        {
            return user;
        }
        return null;
    }

    public async Task<UserRecord> ChangeUsername(string DisplayName, string UID)
    {
        UserRecordArgs args = new UserRecordArgs()
        {
            Uid = UID,
            DisplayName = DisplayName
        };
        return await this._firebaseAuth.UpdateUserAsync(args);
    }
}