using System.Net;
using System.Net.Mail;

namespace EmberFrameworksService.SMTP;

public class SendMail
{
    private static IConfiguration _configuration;

    public SendMail(IConfiguration config)
    {
        _configuration = config;
    }

    public SmtpClient createClient()
    {
        try
        {
            var client = new SmtpClient(_configuration["SMTP_HOST"])
            {
                Port = Int32.Parse(_configuration["SMTP_PORT"]),
                Credentials = new NetworkCredential(_configuration["SMTP_USER"], _configuration["SMTP_PASS"]),
                EnableSsl = true
            };
            return client;
        }
        catch (Exception e){
            Console.WriteLine(e);
        }

        return null;
    }
}