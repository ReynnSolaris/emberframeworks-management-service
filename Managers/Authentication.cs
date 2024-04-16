using System.Diagnostics;
using System.Net;
using EmberFrameworksService.Managers.Firebase;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace EmberFrameworksService.Managers;

public class Authentication
{
    private FirebaseManager _firebaseManager = new();
    private IConfiguration _configuration;

    public Authentication(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Dictionary<string, string> _getUserReq(HttpRequest req)
    {
        Dictionary<string, string> userReq = new Dictionary<string, string>();
        try
        {
            req.Headers.TryGetValue("ID-TOKEN", out var ID_TOKEN);
            req.Headers.TryGetValue("UID", out var UID);
            userReq.Add("UID", !StringValues.IsNullOrEmpty(UID) ? UID : "");
            userReq.Add("ID-TOKEN", !StringValues.IsNullOrEmpty(ID_TOKEN) ? ID_TOKEN : "");
        } catch(Exception e)
        {
            userReq.Add("UID", "");
            userReq.Add("ID-TOKEN", "");
        }
        return userReq;
    }
    public async Task<bool> IsAuthenticatedUser(HttpRequest request)
    {
        string? IpAddress = GetClientIPAddress(request);
        if (IpAddress == "127.0.0.1" || IpAddress == "192.168.1.128")
        {
            return true;
        }
        Dictionary<string, string> userReq = _getUserReq(request);
        Debug.WriteLine($"{userReq["UID"]} - {userReq["ID-TOKEN"]}");
        if (String.IsNullOrEmpty(userReq["UID"]) || String.IsNullOrEmpty(userReq["ID-TOKEN"]))
        {
            return false;
        }
        return await _firebaseManager.VerifyToken(userReq["UID"], userReq["ID-TOKEN"]);
    }

    public string? GetClientIPAddress(HttpRequest req)
    { 
        var remoteIpAddress = req.HttpContext.Connection.RemoteIpAddress?.ToString();
        return remoteIpAddress;
    }

    public bool IsAuthenticatedAPIRequest(HttpRequest req)
    {
        try
        {
            req.Headers.TryGetValue("x-api-key", out var api_key);
            req.Headers.TryGetValue("uid", out var UID);
            UID = !StringValues.IsNullOrEmpty(UID) ? UID : "";
            api_key = !StringValues.IsNullOrEmpty(api_key) ? api_key : "";
            var sec = _configuration.GetSection("api_keys")[UID];
            if (!sec.IsNullOrEmpty())
            {
                if (sec.Equals(api_key))
                    return true;
            }
        } catch(Exception e){ }
        return false;
    }
}