using System.Net;
using System.Net.Mail;

namespace Shopping_Tutorial.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ducthinh14122003@gmail.com", "xqoadedmuwlhtouk")
            };

            return client.SendMailAsync(
                new MailMessage(from: "ducthinh14122003@gmail.com",
                                to: email,
                                subject,
                                message));
        }
    }
}
