using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace OnlineExam.Helpers
{
    public static class EmailGateway
    {
        public static bool SendOTPEmail(string receiver, string otp)
        {
            try
            {
                var senderEmailAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
                var senderDisplayName = ConfigurationManager.AppSettings["SenderDisplayName"];
                var senderEmailPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];

                var senderEmail = new MailAddress(senderEmailAddress, senderDisplayName);
                var receiverEmail = new MailAddress(receiver, "Receiver");

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, senderEmailPassword)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = "OTP",
                    Body = "Email confirmatoin otp-" + otp
                })
                {
                    smtp.Send(mess);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool SendEmail(string receiver, string subject,string body)
        {
            try
            {
                var senderEmailAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
                var senderDisplayName = ConfigurationManager.AppSettings["SenderDisplayName"];
                var senderEmailPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];

                var senderEmail = new MailAddress(senderEmailAddress, senderDisplayName);
                var receiverEmail = new MailAddress(receiver, "Receiver");

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, senderEmailPassword)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml=true
                })
                {
                    smtp.Send(mess);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}