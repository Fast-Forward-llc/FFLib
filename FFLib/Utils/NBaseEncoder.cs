using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Utils
{
    public static class NBaseEncoder
    {
        /// <summary>
        /// encodes unsigned 64bit Integer to Base N String using supplied character set
        /// </summary>
        /// <param name="value">UInt64 Value to encode</param>
        /// <param name="charSet">Character set to use for encoding</param>
        /// <returns></returns>
        public static string Encode(UInt64 value, char[] charSet)
        {
            // 64 is the worst case buffer size for base 2 and UInt64.MaxValue
            int i = 64;
            char[] buffer = new char[i];
            UInt64 targetBase = (UInt64)charSet.Length;

            do
            {
                buffer[--i] = charSet[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            char[] result = new char[64 - i];
            Array.Copy(buffer, i, result, 0, 64 - i);

            return new string(result);
        }

        /// <summary>
        /// encodes unsigned 64bit Integer to Base34 String using 0-9 and uppercase A-Z except O,I 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeUpperAlphaNumeric(UInt64 value)
        {
            var charset = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','J','K','L','M','N','P','Q','R','S','T','U','V','W','X','Y','Z'};

            return NBaseEncoder.Encode(value,charset);
        }
    }
}
