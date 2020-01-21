using System;
using System.Collections.Generic;
using System.Text;

namespace NtcBackup
{
    class BackupInfo
    {
        public string Database { get; set; }
        public string FileName { get; private set; }
        private string _path;

        public string Path
        {
            get { return _path; }
            set 
            { 
                _path = value;
                FileName = System.IO.Path.GetFileName(value);
            }
        }

        public bool Status { get; set; } = false;
        public string Info { get; set; }
    }
}
