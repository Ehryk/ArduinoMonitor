using System;
using System.Net.Mail;

namespace ArduinoMonitor
{
    public static class Email
    {
        public static string SMTPServer;
        public static int SMTPPort = 25;
        public static string FromAddress;

        /// <summary>
        /// Sends an email - attachment is not required
        /// </summary>
        /// <param name="subject">subject line text</param>
        /// <param name="body">body text of the email</param>
        /// <param name="recipientEmailAddress">email address to send to - comma delimited list</param>
        /// <param name="fromEmailAddress">email address to send from</param>
        public static bool SendEmail(string subject, string body, string recipientEmailAddress, string fromEmailAddress = null)
        {
            string from = fromEmailAddress ?? FromAddress;

            //The smtp server used to send the email is in the web.config
            SmtpClient client = new SmtpClient(SMTPServer, SMTPPort);

            MailMessage msg = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(from),
                Subject = subject,
                Body = body
            };

            msg.To.Add(recipientEmailAddress);
            
            try
            {
                client.Send(msg);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
                return false;
            }

            return true;
        }
    }
}