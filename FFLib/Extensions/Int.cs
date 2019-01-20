using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Extensions
{
    public static class IntExtension
    {
        public static int IfNull(this int? self, int Default)
        {
            if (self == null) return 0;
            else return self.Value;
        }

        public static string Join(this int[] list, char delimiter)
        {
            if (list == null || list.Length == 0) return string.Empty;
            System.Text.StringBuilder result = new StringBuilder();
            result.Append(list[0].ToString());
            for (int i = 1; i < list.Length; i++)
                result.Append(delimiter + list[i].ToString());
            return result.ToString();
        }

        public static int IndexOf(this int[] list, int value)
        {
            if (list == null || list.Length == 0) return -1;
            for (var i = 0; i < list.Length; i++) if (list[i] == value) return i;
            return -1;
        }


            /// <summary>
            /// Parse a delimited string of integers into an array of intergers.
            /// Values that cannot be parsed to an integer are set to 0.
            /// </summary>
            /// <param name="self"></param>
            /// <param name="delimiter"></param>
            /// <returns></returns>
        public static int[] ParseIntArray(this string self,char delimiter = '|')
        {

            if (self == null || self.Length == 0) return new int[] { };
            string[] list = self.Split(new char[] { delimiter });
            int[] result = new int[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                int r = 0;
                int.TryParse(list[i], out r);
                result[i] = r;
            }
            return result;
        }
    }
}
