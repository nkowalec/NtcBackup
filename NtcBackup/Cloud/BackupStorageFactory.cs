using System;
using System.Collections.Generic;
using System.Text;

namespace NtcBackup.Cloud
{
    static class BackupStorageFactory
    {
        public static IBackupStorage GetStorage(string storageProvider)
        {
            switch (storageProvider.ToUpper())
            {
                case "DROPBOX":
                case "DROPBOXPROVIDER":
                case "DROPBOXSTORAGE":
                    return new DropboxStorage();
            }

            return null;
        }
    }
}
