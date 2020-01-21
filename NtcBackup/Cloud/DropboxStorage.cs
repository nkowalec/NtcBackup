using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NtcBackup.Cloud
{
    class DropboxStorage : IBackupStorage
    {
        public async Task<string> GetToken()
        {
            Console.WriteLine("Za chwilę otworzona zostanie przeglądarka na stronie Dropbox w celu utworzenia kodu autoryzacyjnego.");
            Console.WriteLine("Zezwól aplikacji na dostęp do twojego konta dropbox i");

            var builder = new ConfigurationBuilder().AddUserSecrets<Secrets.DropboxSecrets>();
            var secret = builder.Build().GetSection(nameof(Secrets.DropboxSecrets)).Get<Secrets.DropboxSecrets>();

            var q = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, clientId: secret.AppKey, redirectUri: (string)null);
            Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = q.ToString() });
            Console.Write("Wprowadź kod autoryzacyjny: ");
            string code = Console.ReadLine();
            var auth = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, secret.AppKey, secret.AppSecret);

            return auth.AccessToken;
        }

        public async Task Upload(string path, object accessToken)
        {
            string fileName = Path.GetFileName(path);

            using (DropboxClient client = new DropboxClient((string)accessToken))
            {
                using (var stream = File.OpenRead(path)) 
                {
                    var data = await client.Files.UploadAsync(
                        "/" + fileName,
                        mode: WriteMode.Overwrite.Instance,
                        body: stream);
                }
            }
        }
    }
}
