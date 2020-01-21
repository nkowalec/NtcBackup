using Microsoft.Extensions.Configuration;
using NtcBackup.Cloud;
using NtcBackup.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NtcBackup
{
    class Program
    {
        static void Main(string[] args)
        {            
            if (args.Any(x => x.ToLower() == "--config"))
            {
                string configCustomPath = args.GetArgValue("--config");
                if (String.IsNullOrEmpty(configCustomPath))
                {
                    Console.WriteLine("Nieprawidłowa wartość parametru --config");
                    return;
                }
                Configuration.ConfigurationPath = configCustomPath;
            }

            if (args.Any(x => x.ToLower() == "--create-example-config"))
            {
                Configuration.CreateExampleConfigFile();
                if (args.Length > 1) Console.WriteLine("Używając parametru --create-example-config inne parametry są ignorowane.");
                return;
            }

            if(args.Any(x => x.ToLower() == "--authorize"))
            {
                string providerName = args.GetArgValue("--authorize");
                if (String.IsNullOrEmpty(providerName))
                {
                    Console.WriteLine("Brak wartości parametru --authorize.");
                    return;
                }
                var config = Configuration.Get();
                var provider = config.UploadProviders.FirstOrDefault(x => x.Name == providerName);
                if(provider == null)
                {
                    Console.WriteLine($"Nie znaleziono providera o nazwie {providerName}");
                    return;
                }

                Console.WriteLine("Autoryzacja usług chmurowych zapisu kopii zapasowych...");
                var storage = BackupStorageFactory.GetStorage(provider.ProviderClass);                
                var token = storage.GetToken().GetAwaiter().GetResult();
                Console.WriteLine("Twój token to: " + token);
                provider.Token = token;
                config.Update();
                Console.WriteLine("Token został zapisany w konfiguracji aplikacji.");
                return;
            }

            if(args.Any(x => x.ToLower() == "--test-mail"))
            {
                new SmtpBackupInfo(new BackupInfo[0]).Send();
                return;
            }

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            if (args.Any(x => x.ToLower() == "--backup"))
            {
                Application application = new Application();
                var app = Task.Run(application.Run);
                app.Wait();
            }
            else
            {
                WriteHelp();
            }
        }

        static void WriteHelp()
        {
            Console.WriteLine("TODO: Napisać helpa... Najlepiej bezpośrednio na github.");
        }
    }
}
