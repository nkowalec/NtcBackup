using System;
using System.Collections.Generic;
using System.Text;

namespace NtcBackup.Config
{
    public class Database
    {
        public string Name { get; set; }
        public string SqlServerName { get; set; }
        public bool Zip { get; set; }
        public string ZipPassword { get; set; }
        public bool Upload { get; set; }
        public string UploadProvider { get; set; }
        public bool DeleteAfterUpload { get; set; }
    }
}
