using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NtcBackup
{
    class ZipFile
    {
        public string Create(string path, bool deleteSource, string zipPassword)
        {
            string directory = Path.GetDirectoryName(path);
            string sourceFileName = Path.GetFileName(path);
            string file = Path.GetFileNameWithoutExtension(path) + ".zip";
            string zipPath = Path.Combine(directory, file);

            using(FileStream stream = File.Create(zipPath))
            {
                using(var zipStream = new ZipOutputStream(stream))
                {
                    if (!String.IsNullOrEmpty(zipPassword))
                        zipStream.Password = zipPassword;

                    zipStream.SetLevel(3);
                    zipStream.PutNextEntry(new ZipEntry(sourceFileName));

                    byte[] buffer = new byte[4096];
                    using (var fileStream = File.OpenRead(path))
                        StreamUtils.Copy(fileStream, zipStream, buffer);

                    zipStream.CloseEntry();
                }
            }

            if (deleteSource)
                File.Delete(path);

            return zipPath;
        }
    }
}
