namespace EmberFrameworksService.Models.Roblox;

public class Mail
{
    public int id { get; set; }
    public string title { get; set; }
    public string body { get; set; }
    public Dictionary<string, int> attachments { get; set; }
}