using NtcBackup.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace NtcBackup
{
    class SmtpBackupInfo
    {
        public SmtpBackupInfo(IEnumerable<BackupInfo> infos)
        {
            Infos = infos;
            config = Configuration.Get();
        }

        public IEnumerable<BackupInfo> Infos { get; }
        private Configuration config;

        public void Send()
        {
            using (SmtpClient client = new SmtpClient(config.Smtp.Server, config.Smtp.Port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(config.Smtp.User, config.Smtp.Password);
                
                using(var message = CreateMailMessage())
                {
                    client.Send(message);
                }
            }
        }

        private MailMessage CreateMailMessage()
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(config.Smtp.From, "NtcBackup");
            m.To.Add(config.Smtp.To);
            m.Subject = "Kopia zapasowa - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            m.IsBodyHtml = true;
            //m.Body = GetHtmlBody();

            m.AlternateViews.Add(GetAlternateView());

            return m;
        }

        private AlternateView GetAlternateView()
        {
            MemoryStream checkStream = new MemoryStream(Resources.check);
            LinkedResource check = new LinkedResource(checkStream);
            check.ContentId = "check.png";
            check.ContentType = new ContentType("image/png");

            MemoryStream errorStream = new MemoryStream(Resources.error);
            LinkedResource error = new LinkedResource(errorStream);
            error.ContentId = "error.png";
            error.ContentType = new ContentType("image/png");

            AlternateView view = AlternateView.CreateAlternateViewFromString(GetHtmlBody(), Encoding.UTF8, MediaTypeNames.Text.Html);
            view.LinkedResources.Add(check);
            view.LinkedResources.Add(error);

            return view;
        }

        private string GetHtmlBody()
        {
            string html = "<html>" +
                "<head>" +
                "</head>" +
                "<style> body { font-family: sans-serif; } table { border: 1px solid black; border-collapse: collapse; } td { padding: 5px 25px 5px 25px; border: 1px solid black; }</style>" +
                "<body>";

            html += "<h2>Status wykonania kopii zapasowych baz danych</h2>";
            html += "<table>";

            foreach(var info in Infos)
            {
                html += "<tr>";
                html += "<td>";
                html += info.Database;
                html += "</td>";
                html += "<td>";
                html += $"<img src='cid:{(info.Status ? "check.png" : "error.png")}' alt='{(info.Status ? "OK" : "ERROR")}' />";
                html += "</td>";
                html += "</tr>";
            }

            html += "</table>";
            html += "<div id='details'>";
            html += "<br />";
            html += "<h3>Szczegóły wykonywania kopii zapasowych</h3>";
            html += "<table>";

            foreach(var info in Infos)
            {
                html += "<tr>";
                html += "<td>";
                html += $"<b>{info.Database}</b><br />";
                html += $"<p>{info.Info.Replace("\n", "<br />")}</p>";
                html += "</td>";
                html += "</tr>";
            }

            html += "</table>";
            html += "</div>";
            html += "</body></html>";
            return html;
        }
    }
}
