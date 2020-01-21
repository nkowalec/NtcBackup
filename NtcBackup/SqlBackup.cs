using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NtcBackup
{
    class SqlBackup
    {
        public string Backup(NtcBackup.Config.Database db)
        {
            var conf = NtcBackup.Config.Configuration.Get();
            var serverConfig = conf.SqlServers.FirstOrDefault(x => x.Name == db.SqlServerName);

            if (serverConfig == null) throw new Exception($"Nie znaleziono konfiguracji serwera o nazwie {db.SqlServerName}");

            string tempFile = Path.Combine(conf.BackupTemporaryDirectory, $"{db.Name}_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.bak");
            var tempFileInfo = new FileInfo(tempFile);
            if (tempFileInfo.Exists)
                tempFileInfo.Delete();

            ServerConnection connection = new ServerConnection(serverConfig.Address, serverConfig.User, serverConfig.Password);
            Server server = new Server(connection);
            BackupDeviceItem device = new BackupDeviceItem(tempFile, DeviceType.File);
            Backup backup = new Backup
            {
                Action = BackupActionType.Database,
                BackupSetDescription = $"{db.Name} - full",
                BackupSetName = $"{db.Name} - backup",
                Database = db.Name,
                Incremental = false,
                LogTruncation = BackupTruncateLogType.Truncate
            };

            backup.Devices.Add(device);
            backup.SqlBackup(server);

            return tempFile;
        }
    }
}
