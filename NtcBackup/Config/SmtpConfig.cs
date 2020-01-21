using System;
using System.Collections.Generic;
using System.Text;

namespace NtcBackup.Config
{
    public class SmtpConfig
    {
        public bool Active { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
