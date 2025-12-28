using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models;
using Serilog;

namespace RentCarSystem.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings; // appsettings jsondan aldık bu ksımı

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                Log.Information("Sending email to: {To}, Subject: {Subject}", to, subject);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort,             // smtp bağlantısı kuruyor mailkit
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);   
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Log.Information("Email sent successfully to: {To}", to);
            }
            catch (Exception ex)
            {
                Log.Error(ex, " Failed to send email to: {To}", to);
                throw;
            }
        }

        public async Task SendVerificationEmailAsync(string to, string token)
        {
            var subject = "Email Verification - RentCar System";
            var body = GetVerificationEmailTemplate(token);
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string to, string token)
        {
            var subject = "Password Reset - RentCar System";
            var body = GetPasswordResetEmailTemplate(token);
            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string to, string userName)
        {
            var subject = "Welcome to RentCar System! ";
            var body = GetWelcomeEmailTemplate(userName);
            await SendEmailAsync(to, subject, body);
        }

        private string GetPasswordResetEmailTemplate(string token)      
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #FF5722; color: white; padding: 20px; text-align: center; }}
                        .content {{ background: #f9f9f9; padding: 30px; margin: 20px 0; }}
                        .token-box {{ 
                            background: white; 
                            border: 2px dashed #FF5722; 
                            padding: 15px; 
                            margin: 20px 0;
                            font-family: monospace;
                            word-break: break-all;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'><h1>🔐 Password Reset</h1></div>
                        <div class='content'>
                            <h2>Reset Your Password</h2>
                            <p>Use this token to reset your password:</p>
                            <div class='token-box'><strong>Token:</strong><br>{token}</div>
                            <p>⚠️ This token expires in 1 hour.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetVerificationEmailTemplate(string token)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ background: #f9f9f9; padding: 30px; }}
                        .token-box {{ 
                            background: white; 
                            border: 2px dashed #4CAF50; 
                            padding: 15px; 
                            margin: 20px 0;
                            font-family: monospace;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'><h1>📧 Email Verification</h1></div>
                        <div class='content'>
                            <h2>Verify Your Email</h2>
                            <div class='token-box'><strong>Token:</strong><br>{token}</div>
                            <p>⏰ Expires in 24 hours.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetWelcomeEmailTemplate(string userName)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <body style='font-family: Arial;'>
                    <div style='max-width: 600px; margin: 0 auto;'>
                        <div style='background: #2196F3; color: white; padding: 20px; text-align: center;'>
                            <h1>Welcome {userName}! 🚗</h1>
                        </div>
                        <div style='padding: 30px;'>
                            <p>Welcome to RentCar System!</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}