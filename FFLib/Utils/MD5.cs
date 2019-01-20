using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Utils
{
    public class MD5
    {

        public static string ComputeHash(string clearText)
        {
            if (clearText == null) return null;
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(clearText);

            return ComputeHash(textBytes);

        }
        public static string ComputeHash(byte[] clearText)
        {
            var hash = MD5.ComputeHashBytes(clearText);
            System.Text.StringBuilder ret = new System.Text.StringBuilder(40);
            foreach (byte a in hash) ret.Append(a.ToString("x2"));
            return ret.ToString();

        }

        public static byte[] ComputeHashBytes(byte[] clearText){
            if (clearText == null) return null;

            System.Security.Cryptography.MD5CryptoServiceProvider MD5;
            MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            return MD5.ComputeHash(clearText);
        }

        public static byte[] ComputeHashBytes(string clearText)
        {
            if (clearText == null) return null;
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(clearText);

            return ComputeHashBytes(textBytes);

        }
    };
}
