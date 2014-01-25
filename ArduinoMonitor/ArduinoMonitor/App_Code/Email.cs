using System;
using System.Net.Mail;

namespace ArduinoMonitor
{
    public class Email
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="tempFahrenheit">Temperature, in Fahrenheit</param>
        public static bool SendEmail(float tempFahrenheit)
        {
            return SendEmail("It's Cold - " + tempFahrenheit + "°F", "Brrrrr", "", "");
        }

        /// <summary>
        /// Sends an email - attachment is not required
        /// </summary>
        /// <param name="subject">subject line text</param>
        /// <param name="body">body text of the email</param>
        /// <param name="recipientEmailAddress">email address to send to - comma delimited list</param>
        /// <param name="fromEmailAddress">email address to send from</param>
        public static bool SendEmail(string subject, string body, string recipientEmailAddress, string fromEmailAddress)
        {
            //The smtp server used to send the email is in the web.config
            SmtpClient client = new SmtpClient("localhost", 25);

            MailMessage msg = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(fromEmailAddress),
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