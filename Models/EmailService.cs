using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Options;
using WS_2_0;

namespace WS_2_0.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public void SendEmail(string to, string subject, string htmlBody)
        {
            
                var message = new MailMessage
                {
                    From = new MailAddress(_settings.From),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = htmlBody
                };

                message.To.Add(to);

                // Vista alternativa HTML
                message.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        htmlBody,
                        Encoding.UTF8,
                        MediaTypeNames.Text.Html
                        )
                );

                using var smtp = new SmtpClient
                {
                    Host = _settings.SmtpServer,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        _settings.User,
                        _settings.Password
                        )
                };
                try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
