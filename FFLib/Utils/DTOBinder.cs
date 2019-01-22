/*******************************************************
 * Project: FFLib V1.0
 * Title: DTOBinder.cs
 * Author: Phillip Bird of Fast Forward,LLC
 * Copyright © 2012 Fast Forward, LLC. 
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * Use of any component of FFLib requires acceptance and adhearance 
 * to the terms of either the MIT License or the GNU General Public License (GPL) Version 2 exclusively.
 * Notification of license selection is not required and will be infered based on applicability.
 * Contributions to FFLib requires a contributor grant on file with Fast Forward, LLC.
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using FFLib.Extensions;

namespace FFLib
{
    public class DTOBinder
    {

        #region options

        public class Options
        {
            public bool SkipNulls = false;
            public bool SkipZeroInts = false;
        }

        #endregion

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        ///<returns>Returns the TargetObject</returns>
        static public T Bind<T>(Object sourceObj, T targetObj, string[] ignore)
        {
            DTOBinder.Bind(sourceObj, null, null, targetObj, null, null, ignore, null);
            return targetObj;
        }

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        /// <param name="skipNulls">Skips binding null values in source object to target. target field/property remains unchanged when source field/property is null.</param>
        ///<returns>Returns the TargetObject</returns>
        static public T Bind<T>(Object sourceObj, T targetObj, string[] ignore, Options options)
        {
            DTOBinder.Bind(sourceObj, null, null, targetObj, null, null, ignore, options);
            return targetObj;
        }

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        static public void Bind(Object sourceObj, Object targetObj, string[] ignore)
        {
            DTOBinder.Bind(sourceObj, null, null, targetObj, null, null, ignore, null);
        }

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        /// <param name="skipNulls">Skips binding null values in source object to target. target field/property remains unchanged when source field/property is null.</param>
        static public void Bind(Object sourceObj, Object targetObj, string[] ignore, Options options)
        {
            DTOBinder.Bind(sourceObj, null, null, targetObj, null, null, ignore, options);
        }

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  and the source propery name is prefixed by 'Prefix'
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="prefix">Match Target property names with this prefix to Source property names without this prefix</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        static public void Bind(Object sourceObj, Object targetObj, string prefix, string[] ignore)
        {
            DTOBinder.Bind(sourceObj, null, null, targetObj, prefix, null, ignore, null);
        }
        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  and the source propery name is prefixed by 'Prefix'
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="prefix">Match Source property names with this prefix to Target property names without this prefix</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        static public void Bind(Object sourceObj, string prefix, Object targetObj, string[] ignore)
        {
            DTOBinder.Bind(sourceObj, prefix, null, targetObj, null, null, ignore, null);
        }
        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  and the source propery name is prefixed by 'Prefix'
        ///  and the source property name has a suffix of 'Suffix'
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="prefix">Match Source property names with this prefix to Target property names without this prefix</param>
        /// <param name="suffix">Match Source property names with this suffix to Target property names without this suffix</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        static public void Bind(Object sourceObj, string prefix, string suffix, Object targetObj, string[] ignore)
        {
            DTOBinder.Bind(sourceObj, prefix, suffix, targetObj, null, null, ignore, null);
        }

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  and the source propery name is prefixed by 'Prefix'
        ///  and the source property name has a suffix of 'Suffix'
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="obj">object instance to bind vaues to</param>
        /// <param name="prefix">Match Target property names with this prefix to Source property names without this prefix</param>
        /// <param name="suffix">Match Target property names with this suffix to Source property names without this suffix</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        static public void Bind(Object sourceObj, Object targetObj, string prefix, string suffix, string[] ignore)
        {
            DTOBinder.Bind(sourceObj, null, null, targetObj, prefix, suffix, ignore, null);
        }

        /// <summary>
        /// Sets the value of properties in the target object with values from the source object where the propery names match.
        ///  and the source propery name is prefixed by 'Prefix'
        ///  and the source property name has a suffix of 'Suffix'
        /// </summary>
        /// <param name="sourceObj">list of controls to attempt to bind</param>
        /// <param name="targetObj">object instance to bind vaues to</param>
        /// <param name="sprefix">Match Source property names with this prefix to Target property names without this prefix</param>
        /// <param name="ssuffix">Match Source property names with this suffix to Target property names without this suffix</param>
        /// <param name="tprefix">Match Target property names with this prefix to Source property names without this prefix</param>
        /// <param name="tsuffix">Match Target property names with this suffix to Source property names without this suffix</param>
        /// <param name="ignore">String array of target properties by name that are not to bound from the source i.e. ignore</param>
        static public void Bind(Object sourceObj, string sprefix, string ssuffix, Object targetObj, string tprefix, string tsuffix, string[] ignore, Options options)
        {
            if (sourceObj == null || targetObj == null) { return; }
            if (options == null) options = new Options();
            try
            {
                //index ignore fields
                Dictionary<string, byte> ignoreList = new Dictionary<string, byte>();
                if (ignore != null && ignore.Length > 0)
                {
                    foreach (string fieldName in ignore)
                        ignoreList.Add(fieldName, 0);
                }
                Type srcType;
                string srcPropName;
                Dictionary<string, PropertyInfo> targetPropList = new Dictionary<string, PropertyInfo>();
                Type targetType = targetObj.GetType();
                PropertyInfo[] targetPropArray = targetType.GetProperties();

                //load target properties into a dictionary by name
                foreach (PropertyInfo p in targetPropArray)
                {
                    //check for mapping attributes
                    var attribs = p.GetCustomAttributes(false);
                    if (attribs != null)
                        foreach (var attr in attribs)
                            if (attr is Attributes.MapsToAttribute)
                            {
                                //add property to list indexed by alias name
                                targetPropList.AddOrReplace(((Attributes.MapsToAttribute)attr).PropertyName, p);
                                //skip to next property
                                continue;
                            }
                    targetPropList.AddOrReplace(p.Name, p);
                }

                //load source properties
                srcType = sourceObj.GetType();
                PropertyInfo[] srcPropArray = srcType.GetProperties();

                //loop thru each source property and bind to target object if a match
                foreach (PropertyInfo sProp in srcPropArray)
                {
                    srcPropName = targetPropertyName(sProp, sprefix, ssuffix, tprefix, tsuffix);
                    //if target propertyname is in ignore list skip to next property
                    if (ignoreList.ContainsKey(srcPropName)) continue;

                    //check for alternate mapping PropertyName
                    var attribs = sProp.GetCustomAttributes(false);
                    if (attribs != null)
                        foreach (var attr in attribs)
                            if (attr is Attributes.MapsToAttribute)
                            {
                                //use alternate property name
                                srcPropName = ((Attributes.MapsToAttribute)attr).PropertyName;
                            }
                    //if expected property exists on target object bind the source value to it
                    if (targetPropList.ContainsKey(srcPropName))
                    {
                        object value = null;
                        PropertyInfo prop = targetPropList[srcPropName];
                        if (!prop.CanWrite) { continue; }
                        //only supports valuetypes, strings and guid. (your basic primitive types)
                        if (!prop.PropertyType.IsValueType && prop.PropertyType != typeof(string) && prop.PropertyType != typeof(Guid)) { continue; }


                        try
                        {
                            value = sProp.GetValue(sourceObj, null);
                            if (options.SkipNulls && value == null) continue;
                            if (options.SkipZeroInts && value != null)
                            {
                                if (value is int && (int)value == 0) continue;
                                if (sProp.PropertyType.IsEnum && (
                                    (Enum.GetUnderlyingType(value.GetType()) == typeof(Int32) && (Int32)value == 0) ||
                                    (Enum.GetUnderlyingType(value.GetType()) == typeof(byte) && (byte)value == 0) ||
                                    (Enum.GetUnderlyingType(value.GetType()) == typeof(Int16) && (Int16)value == 0) ||
                                    (Enum.GetUnderlyingType(value.GetType()) == typeof(Int64) && (Int64)value == 0) ||
                                    (Enum.GetUnderlyingType(value.GetType()) == typeof(short) && (short)value == 0) 
                                    ))
                                    continue;
                            }
                            DTOBinder._setValue(targetObj, prop, value);
                        }
                        catch (Exception ex)
                        { System.Diagnostics.Debug.WriteLine(ex.ToString()); }
                    }
                }
            }
            catch (Exception ex)
            { System.Diagnostics.Debug.WriteLine(ex.ToString()); }

        }


        /// <summary>
        /// Produce expected target property name after source prefix and suffix has been stripped and target prefix and suffix is applied
        /// </summary>
        /// <param name="sProp">Source Property as PropertyInfo</param>
        /// <param name="sprefix">Source Prefix</param>
        /// <param name="ssuffix">Source Suffix</param>
        /// <param name="tprefix">Target Prefix</param>
        /// <param name="tsuffix">Target Suffix</param>
        /// <returns>Target Property Name</returns>
        static private string targetPropertyName(PropertyInfo sProp, string sprefix, string ssuffix, string tprefix, string tsuffix)
        {
            string srcPropName = sProp.Name;

            try
            {
                if (!IsNullOrWhiteSpace(sprefix) && sProp.Name != null && sProp.Name.StartsWith(sprefix)) { srcPropName = srcPropName.Substring(sprefix.Length); }
                if (!IsNullOrWhiteSpace(ssuffix) && sProp.Name != null && sProp.Name.EndsWith(ssuffix)) { srcPropName = srcPropName.Remove(srcPropName.Length - ssuffix.Length); }
            }
            catch (Exception ex)
            { System.Diagnostics.Debug.WriteLine(ex.ToString()); }

            try
            {
                if (!IsNullOrWhiteSpace(tprefix) && sProp.Name != null) { srcPropName = tprefix + srcPropName; }
                if (!IsNullOrWhiteSpace(tsuffix) && sProp.Name != null) { srcPropName = srcPropName + tsuffix; }
            }
            catch (Exception ex)
            { System.Diagnostics.Debug.WriteLine(ex.ToString()); }

            return srcPropName;
        }

        static private void _setValue(Object obj, PropertyInfo prop, Object value)
        {
            string propTypeName = prop.PropertyType.Name;
            if (prop.PropertyType.IsGenericType) {
                if (value == null) { prop.SetValue(obj, null, null); return; }
                propTypeName = Nullable.GetUnderlyingType(prop.PropertyType).Name; 
            }
            if (prop.PropertyType.IsClass && value == null) { prop.SetValue(obj, null, null); return; }
            switch (propTypeName.ToLower())
            {
                case "string": prop.SetValue(obj, Convert.ToString(value), null); break;
                case "char": prop.SetValue(obj, Convert.ToChar(value), null); break;
                case "byte": prop.SetValue(obj, Convert.ToByte(value), null); break;
                case "int": prop.SetValue(obj, Convert.ToInt32(value), null); break;
                case "int16": prop.SetValue(obj, Convert.ToInt16(value), null); break;
                case "int32": prop.SetValue(obj, Convert.ToInt32(value), null); break;
                case "int64": prop.SetValue(obj, Convert.ToInt64(value), null); break;
                case "uint16": prop.SetValue(obj, Convert.ToUInt16(value), null); break;
                case "uint32": prop.SetValue(obj, Convert.ToUInt32(value), null); break;
                case "uint64": prop.SetValue(obj, Convert.ToUInt64(value), null); break;
                case "long": prop.SetValue(obj, Convert.ToInt64(value), null); break;
                case "decimal": prop.SetValue(obj, Convert.ToDecimal(value), null); break;
                case "single": prop.SetValue(obj, Convert.ToSingle(value), null); break;
                case "double": prop.SetValue(obj, Convert.ToDouble(value), null); break;
                case "boolean": prop.SetValue(obj, Convert.ToBoolean(value), null); break;
                case "bool": prop.SetValue(obj, Convert.ToBoolean(value), null); break;
                case "datetime": prop.SetValue(obj, Convert.ToDateTime(value), null); break;
                case "guid": prop.SetValue(obj, new Guid((string)value), null); break;
                case "date": prop.SetValue(obj, Convert.ToDateTime(value), null); break;
                default:
                    if (prop.PropertyType.IsEnum && value is string)
                        value = Enum.Parse(prop.PropertyType, value.ToString());
                    prop.SetValue(obj, value, null); break;
            }
        }

        static private bool IsNullOrWhiteSpace(string str)
        {   
                return string.IsNullOrWhiteSpace(str);
        }
    }
}
