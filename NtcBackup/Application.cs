using NtcBackup.Cloud;
using NtcBackup.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtcBackup
{
    class Application
    {
        public async Task Run()
        {
            Configuration config = Configuration.Get();
            List<BackupInfo> backupInfos = new List<BackupInfo>();
            Stopwatch sw = new Stopwatch();
            Stopwatch lsw = new Stopwatch();
            sw.Start();
            lsw.Start();

            Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Start aplikacji...");
            foreach (Database db in config.Databases)
            {
                BackupInfo info = new BackupInfo { Database = db.Name };
                backupInfos.Add(info);
                StringWriter dbTraceStringWriter = new StringWriter();
                TextWriterTraceListener dbTrace = new TextWriterTraceListener(dbTraceStringWriter);
                Trace.Listeners.Add(dbTrace);

                try
                {
                    Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Tworzenie kopii bazy danych {db.Name}...");
                    lsw.Restart();
                    info.Path = new SqlBackup().Backup(db);
                    Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Tworzenie kopii zakończone ({lsw.ElapsedMilliseconds} ms)...");

                    if (db.Zip)
                    {
                        Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Kompresowanie pliku {info.FileName}...");
                        lsw.Restart();
                        info.Path = new ZipFile().Create(info.Path, true, db.ZipPassword);
                        Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Kompresja zakończona ({lsw.ElapsedMilliseconds} ms).");
                    }

                    if (db.Upload)
                    {
                        var provider = config.UploadProviders.FirstOrDefault(x => x.Name == db.UploadProvider);
                        if(provider == null) throw new Exception($"Provider o nazwie {db.UploadProvider} nieznaleziony");
                        
                        Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Wysyłanie kopii zapasowej...");
                        lsw.Restart();
                        var storage = BackupStorageFactory.GetStorage(provider.ProviderClass);
                        await storage.Upload(info.Path, provider.Token);
                        Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Wysyłanie kopii zakończone ({lsw.ElapsedMilliseconds} ms).");

                        if (db.DeleteAfterUpload)
                        {
                            File.Delete(info.Path);
                            Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Usunięto lokalną kopię zapasową.");
                        }
                    }

                    info.Status = true;
                }
                catch (Exception ex)
                {
                    info.Status = false;
                    Trace.WriteLine(ex.Message);
                }

                Trace.Listeners.Remove(dbTrace);
                info.Info = dbTraceStringWriter.ToString();
                dbTrace.Dispose();
                dbTraceStringWriter.Dispose();
            }

            Trace.WriteLine($"[{sw.ElapsedMilliseconds} ms] Koniec pracy aplikacji...");
            sw.Stop();
            lsw.Stop();

            new SmtpBackupInfo(backupInfos).Send();
        }
    }
}
