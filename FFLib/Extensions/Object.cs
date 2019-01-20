using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns the int value when not null or 0 when int? is null.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int NotNull(this int? self)
        {
            if (self != null) return self.Value;
            return 0;
        }

        /// <summary>
        /// Returns the int value when not null or 0 when int? is null.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static decimal NotNull(this decimal? self)
        {
            if (self != null) return self.Value;
            return 0;
        }

        /// <summary>
        /// Returns the int value when not null or 0 when int? is null.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DateTime NotNull(this DateTime? self)
        {
            if (self != null) return self.Value;
            return DateTime.Parse("1900-01-01 12:00");
        }

        /// <summary>
        /// returns the string value when not null or string.empty when string is null.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>the string or string.empty</returns>
        public static string NotNull(this string self)
        {
            if (self != null) return self;
            return string.Empty;
        }

        /// <summary>
        /// returns the object instance when not null or a new instance when null.
        /// </summary>
        /// <typeparam name="T">Type must have a parameterless constructor</typeparam>
        /// <param name="self"></param>
        /// <returns>The object instance or a new instance</returns>
        public static T NotNull<T>(this T self) where T :class,new()
        {
            if (self != null) return self;
            return new T();
        }

        /// <summary>
        /// returns the object array instance when not null or a new object array instance when null.
        /// </summary>
        /// <typeparam name="T">Type must have a parameterless constructor</typeparam>
        /// <param name="self"></param>
        /// <returns>The object array instance or a new array instance</returns>
        public static T[] NotNull<T>(this T[] self)
        {
            if (self != null) return self;
            return new T[] { };
        }

        /// <summary>
        /// returns the IEnumerable&lt;T> instance when not null or a new instance when null.
        /// </summary>
        /// <typeparam name="T">Type must have a parameterless constructor</typeparam>
        /// <param name="self"></param>
        /// <returns>The IEnumerable&lt;T> instance or a new instance</returns>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> self)
        {
            if (self != null) return self;
            return new T[] { };
        }
    }
}
