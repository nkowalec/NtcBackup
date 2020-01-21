using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace NtcBackup.Config
{
    public class Configuration
    {
        public static string ConfigurationPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "NtcBackup.xml");
        private static Configuration _instance;
        public static Configuration Get()
        {
            if(_instance == null)
                _instance = GetFromFile();
            return _instance;
        }

        public string BackupTemporaryDirectory { get; set; }
        public List<SqlServer> SqlServers { get; set; }
        public List<Database> Databases { get; set; }
        public List<UploadProvider> UploadProviders { get; set; }
        public SmtpConfig Smtp { get; set; }

        public static Configuration GetFromFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            Configuration c = null;

            if (File.Exists(ConfigurationPath))
            {
                using (var stream = File.OpenRead(ConfigurationPath))
                {
                    c = (Configuration)serializer.Deserialize(stream);
                }
            }

            return c;
        }

        public static void CreateExampleConfigFile()
        {
            Configuration c = new Configuration
            {
                BackupTemporaryDirectory = @"C:\TEMP",
                Databases = new List<Database>()
                {
                    new Database
                    {
                        Name = "MySqlDatabase1",
                        SqlServerName = "SqlExpress",
                        DeleteAfterUpload = true,
                        Upload = true,
                        UploadProvider = "Dropbox",
                        Zip = true,
                        ZipPassword = null
                    }
                },
                SqlServers = new List<SqlServer>()
                {
                    new SqlServer
                    {
                        Name = "SqlExpress",
                        Address = @".\SqlExpress",
                        User = "SA",
                        Password = "1234qwerasdfZXCV"
                    }
                },
                UploadProviders = new List<UploadProvider>()
                {
                    new UploadProvider
                    {
                        Name = "Dropbox",
                        ProviderClass = "DropboxStorage",
                        Token = "xxxxxxxxxxxxx"
                    }
                },
                Smtp = new SmtpConfig
                {
                    Active = false,
                    From = "aaa@example.com",
                    To = "bbb@example.com",
                    User = "aaa",
                    Password = "1234qwerasdfZXCV",
                    Server = "stmp.example.com",
                    Port = 587
                }
            };

            c.Update();
        }

        public void Update()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            using(var stream = File.OpenWrite(ConfigurationPath))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
