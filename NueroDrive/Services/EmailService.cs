using System.Net;
using System.Net.Mail;

namespace NueroDrive.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendUnauthorizedAccessNotificationAsync(string userEmail, string carName, DateTime timestamp, string? imageBase64 = null)
        {
            var subject = $"Unauthorized Vehicle Access Attempt - {carName}";
            var body = $@"
                <h2>Unauthorized Vehicle Access Attempt Detected</h2>
                <p>An unauthorized person attempted to access your vehicle.</p>
                <p><strong>Vehicle:</strong> {carName}</p>
                <p><strong>Time:</strong> {timestamp.ToString("yyyy-MM-dd HH:mm:ss")}</p>
                {(imageBase64 != null ? "<p><strong>Person Image:</strong></p><p><img src='cid:unauthorizedImage' alt='Unauthorized person' style='max-width: 400px;' /></p>" : "")}
                <p>Please contact support if you need assistance.</p>
                <p>Regards,<br>NueroDrive Team</p>
            ";

            await SendEmailAsync(userEmail, subject, body, imageBase64);
        }

        private async Task SendEmailAsync(string to, string subject, string body, string? imageBase64 = null)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");
                
                var smtpClient = new SmtpClient
                {
                    Host = smtpSettings["SmtpHost"] ?? "smtp.gmail.com",
                    Port = int.Parse(smtpSettings["SmtpPort"] ?? "587"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        smtpSettings["SmtpUsername"] ?? "[EMAIL_USERNAME_MISSING]",
                        smtpSettings["SmtpPassword"] ?? "[EMAIL_PASSWORD_MISSING]"
                    )
                };

                using var message = new MailMessage(
                    from: smtpSettings["SenderEmail"] ?? "[SENDER_EMAIL_MISSING]",
                    to: to
                )
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                // Add image attachment if provided
                if (!string.IsNullOrEmpty(imageBase64))
                {
                    try
                    {
                        var imageBytes = Convert.FromBase64String(imageBase64);
                        using var ms = new MemoryStream(imageBytes);
                        var attachment = new Attachment(ms, "unauthorized_person.jpg", "image/jpeg");
                        // Set content ID for HTML embedding
                        attachment.ContentId = "unauthorizedImage";
                        // Make sure the image is displayed in the email body
                        attachment.ContentDisposition.Inline = true;
                        message.Attachments.Add(attachment);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to attach image to the email");
                    }
                }

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
            }
        }
    }
} 