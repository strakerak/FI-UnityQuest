using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;

public class EmailFactory : MonoBehaviour
{
    ///public InputField bodyMessage;
    ///public InputField recipientEmail;
    

    public void SendEmail()
    {

        string path1 = Application.persistentDataPath + "/log.txt";
        string[] logText = File.ReadAllLines(path1);
        ///logText = logText.Select(x => x.TrimEnd("\n") + "\n").ToArray();
        ///
        for (int i = 0; i < logText.Length; i++)
        {
            if (!logText[i].EndsWith("\n"))
            {
                logText[i] += "\n";
            }
        }

        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("smtp-mail.outlook.com");
        SmtpServer.Timeout = 10000;
        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        SmtpServer.UseDefaultCredentials = false;
        SmtpServer.Port = 587;

        mail.From = new MailAddress("uhmrilabrelay@outlook.com");
        mail.To.Add(new MailAddress("stawakko@cougarnet.uh.edu"));

        mail.Subject = "FI3D Client Application Log";
        ///mail.Subject = "FI3D Client Application Log" + SystemInfo.deviceModel + SystemInfo.deviceName + DateTime.UtcNow;
        ///mail.Body = string.Join(",", logText);
        mail.Body = "This message has been sent from a FI3D client.\n" 
            + "Device Name: " 
            + SystemInfo.deviceName 
            + "\n" + "Device Model: " 
            + SystemInfo.deviceModel
            + "\n Log time: " 
            + DateTime.UtcNow;

        mail.Attachments.Add(new Attachment(Application.persistentDataPath + "/log.txt"));


        SmtpServer.Credentials = new System.Net.NetworkCredential("uhmrilabrelay@outlook.com", "hG5#u%ne*GrQ") as ICredentialsByHost; SmtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };

        Debug.Log(string.Format("{0} | Navigation | Send Email Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        ///App.LogMessage(string.Format("{0} | Navigation | Send Email Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        SmtpServer.Send(mail);
        

        ///App gohesag = new App();
        ///gohesag.onClearLogClick();
        
        
    }
}