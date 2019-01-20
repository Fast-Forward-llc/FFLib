using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Extensions
{
    public static class HashtableExtension
    {
        public static void AddRange(this Hashtable self, string[] values)
        {
            if (values == null || values.Length == 0) return;

            foreach (var i in values) if (!string.IsNullOrEmpty(i)) self.Add(i, i);
        }

        public static void AddRange(this Hashtable self, int[] values)
        {
            if (values == null || values.Length == 0) return;

            foreach (var i in values) self.Add(i, i);
        }
    }
}
