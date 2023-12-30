using System.Diagnostics;
using System.Net;
using EmberFrameworksService.Managers.Firebase;
using Microsoft.Extensions.Primitives;

namespace EmberFrameworksService.Managers;

public class Authentication
{
    private FirebaseManager _firebaseManager = new();
    
    public Dictionary<string, string> _getUserReq(HttpRequest req)
    {
        Dictionary<string, string> userReq = new Dictionary<string, string>();
        try
        {
            req.Headers.TryGetValue("ID_TOKEN", out var ID_TOKEN);
            req.Headers.TryGetValue("UID", out var UID);
            userReq.Add("UID", !StringValues.IsNullOrEmpty(UID) ? UID : "");
            userReq.Add("ID_TOKEN", !StringValues.IsNullOrEmpty(ID_TOKEN) ? ID_TOKEN : "");
        } catch(Exception e)
        {
            userReq.Add("UID", "");
            userReq.Add("ID_TOKEN", "");
        }
        return userReq;
    }
    public async Task<bool> IsAuthenticatedUser(HttpRequest request)
    {
        Dictionary<string, string> userReq = _getUserReq(request);
        Debug.WriteLine($"{userReq["UID"]} - {userReq["ID_TOKEN"]}");
        if (String.IsNullOrEmpty(userReq["UID"]) || String.IsNullOrEmpty(userReq["ID_TOKEN"]))
        {
            return false;
        }
        return await _firebaseManager.VerifyToken(userReq["UID"], userReq["ID_TOKEN"]);
    }

    public string? GetClientIPAddress(HttpRequest req)
    { 
        var remoteIpAddress = req.HttpContext.Connection.RemoteIpAddress?.ToString();
        return remoteIpAddress;
    }
}