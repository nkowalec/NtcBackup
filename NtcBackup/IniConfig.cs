using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NtcBackup
{
    class IniConfig
    {
        private static readonly string IniPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "NtcBackup.ini");
        public static readonly IniConfig Instance = new IniConfig();

        private IniConfig()
        {
            if (File.Exists(IniPath))
                Deserialize();
        }

        public string DB_SERVER { get; set; } = @".\SQLSERVER";
        public string DB_USER { get; set; } = "SA";
        public string DB_PASSWORD { get; set; } = "HASLO1234";
        public string[] DB_NAMES { get; set; } = new string[] { "DB_1", "DB_2" };
        public bool ZIP { get; set; } = true;
        public string TEMP_DIR { get; set; } = @"C:\TEMP";
        public string ZIP_PASSWORD { get; set; } = "";
        public bool UPLOAD_ACTIVE { get; set; } = true;
        public string UPLOAD_PROVIDER { get; set; } = "DropboxStorage";
        public string UPLOAD_TOKEN { get; set; } = "EXAMPLE_TOKEN";
        public bool DELETE_AFTER_UPLOAD { get; set; } = true;
        public bool SMTP_INFO { get; set; } = true;
        public string SMTP_TO { get; set; } = "nkowalec@hotmail.com";
        public string SMTP_FROM { get; set; } = "noreply.nik@gmail.com";

        public void CreateExampleIni()
        {
            Serialize();
        }

        private void Serialize()
        {
            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var file = new FileInfo(IniPath);

            using (var stream = file.CreateText())
            {
                foreach (var prop in props)
                {
                    if (prop.PropertyType.IsArray)
                    {
                        stream.WriteLine($"{prop.Name} = [{String.Join(", ", (object[])prop.GetValue(this))}]");
                    }
                    else
                    {
                        stream.WriteLine($"{prop.Name} = {Convert.ToString(prop.GetValue(this)).Trim()}");
                    }
                }
            }
        }

        private void Deserialize()
        {
            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var file = new FileInfo(IniPath);

            using (StreamReader reader = new StreamReader(file.OpenRead()))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrEmpty(line)) continue; //brak danych w linii
                    if (!line.Contains('=')) continue; //brak separatora
                    var indexOfSplit = line.IndexOf('=');

                    string key = line.Substring(0, indexOfSplit).Trim();
                    string value = line.Substring(indexOfSplit + 1);

                    var prop = props.FirstOrDefault(x => x.Name == key);
                    if (prop == null) continue; //nieprawidłowa nazwa parametru

                    if (prop.PropertyType.IsArray)
                    {
                        string[] values = value.Trim().Trim('[', ']').Split(',').Select(x => x.Trim()).ToArray();
                        prop.SetValue(this, values);
                    }
                    else
                    {
                        prop.SetValue(this, Convert.ChangeType(value.Trim(), prop.PropertyType));
                    }
                }
            }
        }
    }
}
