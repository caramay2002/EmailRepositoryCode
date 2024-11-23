using System;
using System.Net;
using System.Net.Mail;

class Program
{
    static void Main()
    {
        // Email parameters
        string senderEmail = "goodmorning@example.com";
        string senderPassword = "*******";

        // SMTP server details
        string smtpHost = "mail.example.com";
        int smtpPort = 2525; // Port number for the SMTP server
        bool enableSsl = true; // Set it to true if your SMTP server requires SSL/TLS


        // Send to
        string recipientEmail = "example@gmail.com";
        string subject = "Hello, World!";
        string body = "This is the body of the email.";
        

        try
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(senderEmail);
                mailMessage.To.Add(recipientEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.Send(mailMessage);
                }

                Console.WriteLine("Email sent successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending email: " + ex.Message);
        }
    }
}
