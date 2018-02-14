using System;
using System.IO;
using System.Security.Cryptography;

namespace FileSystemAbstraction.Helpers
{
    public class ChecksumHelper
    {
        private static string ByteArrayString(byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", " ");
        
        public static string FromContent(byte[] content)
        {
            using (var md5 = MD5.Create())
                return ByteArrayString(md5.ComputeHash(content));
        }

        public static string FromStream(Stream stream)
        {
            using (var md5 = MD5.Create())
                return ByteArrayString(md5.ComputeHash(stream));
        }
    }
}