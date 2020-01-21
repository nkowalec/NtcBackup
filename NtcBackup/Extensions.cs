using System;
using System.Collections.Generic;
using System.Text;

namespace NtcBackup
{
    static class Extensions
    {
        public static string GetArgValue(this string[] args, string param)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].ToLower() == param.ToLower())
                {
                    if (args.Length < i + 1)
                        return args[i + 1];
                }
            }

            return null;
        }
    }
}
