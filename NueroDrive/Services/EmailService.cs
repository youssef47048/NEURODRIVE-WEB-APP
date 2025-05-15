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
            _logger.LogInformation("Preparing to send unauthorized access email to {Email}", userEmail);
            
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
                var smtpHost = smtpSettings["SmtpHost"];
                var smtpPort = smtpSettings["SmtpPort"];
                var smtpUsername = smtpSettings["SmtpUsername"];
                var smtpPassword = smtpSettings["SmtpPassword"];
                var senderEmail = smtpSettings["SenderEmail"] ?? smtpUsername;
                
                _logger.LogInformation("Email settings: Host={Host}, Port={Port}, Username={Username}, SenderEmail={SenderEmail}", 
                    smtpHost, smtpPort, smtpUsername, senderEmail);
                
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogError("Email settings are missing. Cannot send email.");
                    return;
                }
                
                var smtpClient = new SmtpClient
                {
                    Host = smtpHost,
                    Port = int.Parse(smtpPort ?? "587"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                
                message.To.Add(to);

                // Add image attachment if provided
                if (!string.IsNullOrEmpty(imageBase64))
                {
                    try
                    {
                        // Remove data:image prefix if present
                        string base64Data = imageBase64;
                        if (base64Data.Contains(","))
                        {
                            base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
                        }
                        
                        var imageBytes = Convert.FromBase64String(base64Data);
                        _logger.LogInformation("Image size: {Size} bytes", imageBytes.Length);
                        
                        using var ms = new MemoryStream(imageBytes);
                        var attachment = new Attachment(ms, "unauthorized_person.jpg", "image/jpeg");
                        // Set content ID for HTML embedding
                        attachment.ContentId = "unauthorizedImage";
                        // Make sure the image is displayed in the email body
                        attachment.ContentDisposition.Inline = true;
                        message.Attachments.Add(attachment);
                        _logger.LogInformation("Image attachment added to email");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to attach image to the email");
                    }
                }

                _logger.LogInformation("Sending email to {To}", to);
                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
            }
        }
    }
} 