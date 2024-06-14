using BugTracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BugTracker.Services
{
    public class SendGridService : IEmailSender, IEmailSender<ApplicationUser>
    {
        private readonly string _sendGridKey;
        private readonly string _fromAddress;
        private readonly string _fromName;

        public SendGridService(IConfiguration config)
        {
            _sendGridKey = config["SendGridKey"] ?? Environment.GetEnvironmentVariable("SendGridKey") ?? throw new InvalidOperationException("SendGridKey could not be found.");

            _fromAddress = config["SendGridEmail"] ?? Environment.GetEnvironmentVariable("SendGridEmail") ?? throw new InvalidOperationException("SendGridEmail could not be found.");

            _fromName = config["SendGridName"] ?? Environment.GetEnvironmentVariable("SendGridName") ?? throw new InvalidOperationException("SendGridName could not be found.");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SendGridClient client = new SendGridClient(_sendGridKey);
            EmailAddress from = new EmailAddress(_fromAddress, _fromName);

            List<string> emails = [.. email.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];
            List<EmailAddress> addresses = [.. emails.Select(e => new EmailAddress(e))];

            string plainTextContent = Regex.Replace(htmlMessage, "<[a-zA-Z].*?>", "").Trim();

            SendGridMessage message = MailHelper.CreateSingleEmailToMultipleRecipients(from, addresses, subject, plainTextContent, htmlMessage);

            Response reponse = await client.SendEmailAsync(message);

            if (reponse.IsSuccessStatusCode == false)
            {
                Console.WriteLine("********* EMAIL SERVICE ERROR*********");
                Console.WriteLine(await reponse.Body.ReadAsStringAsync());
                Console.WriteLine("********* EMAIL SERVICE ERROR*********");

                throw new BadHttpRequestException("SendGrid email could not be sent.");
            }
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");

    }
}
