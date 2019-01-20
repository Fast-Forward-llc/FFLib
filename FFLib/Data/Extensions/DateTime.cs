/*******************************************************
 * Project: FFLib V1.0
 * Title: String.cs
 * Author: Phillip Bird of Fast Forward,LLC
 * Copyright © 2012 Fast Forward, LLC. 
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * Use of any component of FFLib requires acceptance and adhearance 
 * to the terms of either the MIT License or the GNU General Public License (GPL) Version 2 exclusively.
 * Notification of license selection is not required and will be infered based on applicability.
 * Contributions to FFLib requires a contributor grant on file with Fast Forward, LLC.
********************************************************/
using System;


namespace FFLib.Extensions
{
    public static class DateTimeExtentions
    {
        public static string ToSqlDateTime(this DateTime self)
        {
            return self.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToSqlDate(this DateTime self)
        {
            return self.ToString("yyyy-MM-dd");
        }

        public static string ToSqlDateTime(this DateTime? self)
        {
            if (!self.HasValue) return null;
            return self.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToSqlDate(this DateTime? self)
        {
            if (!self.HasValue) return null;
            return self.Value.ToString("yyyy-MM-dd");
        }
    }
}
