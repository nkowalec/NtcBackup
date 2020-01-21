using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NtcBackup.Cloud
{
    interface IBackupStorage
    {
        Task Upload(string path, object accessToken);
        Task<string> GetToken();
    }
}
