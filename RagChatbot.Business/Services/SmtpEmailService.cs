using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RagChatbot.Business.Interfaces;

namespace RagChatbot.Business.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var host = _configuration["EmailConfiguration:Host"] ?? Environment.GetEnvironmentVariable("EmailConfiguration__Host");
                var portStr = _configuration["EmailConfiguration:Port"] ?? Environment.GetEnvironmentVariable("EmailConfiguration__Port");
                var username = _configuration["EmailConfiguration:Username"] ?? Environment.GetEnvironmentVariable("EmailConfiguration__Username");
                var password = _configuration["EmailConfiguration:Password"] ?? Environment.GetEnvironmentVariable("EmailConfiguration__Password");

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("[SMTP ERROR]: Email configuration is missing.");
                    return;
                }

                int port = 587;
                if (!string.IsNullOrEmpty(portStr))
                {
                    int.TryParse(portStr, out port);
                }

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(username, "RAG Chatbot System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMTP ERROR]: Failed to send email to {toEmail}. Error: {ex.Message}");
            }
        }
    }
}
