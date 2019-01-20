/*******************************************************
 * Project: FFLib V1.0
 * Title: HTMLEncoder.cs
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
using System.Text;
using System.Reflection;
using FFLib.Attributes;
using Microsoft.Security.Application;

namespace FFLib
{
    public class HtmlEncoder
    {
        
        static public void Encode(Object sourceObj)
        {
            if (sourceObj == null) { return; }
            try
            {
                Type objectType = sourceObj.GetType();
                PropertyInfo[] sourcePropArray = objectType.GetProperties();
                
                //load target properties into a dictionary by name
                foreach (PropertyInfo p in sourcePropArray)
                {
                    //check for mapping attributes
                    var attribs = p.GetCustomAttributes(false);
                    if (attribs != null)
                    {
                        if (attribs.Where(m => m is HtmlEncodingAttribute).Count() == 0  && p.PropertyType == typeof(string))
                        {
                            var value = p.GetValue(sourceObj, null);
#if CLR_V4
                             value = Microsoft.Security.Application.Encoder.HtmlEncode((string)value);
#else
                             value = Microsoft.Security.Application.AntiXss.HtmlEncode((string)value);
#endif
                            _setValue(sourceObj, p, value);
                        }
                        foreach (var attr in attribs)
                        { 
                            if (attr is HtmlEncodingAttribute)
                            {
                                //add property to list indexed by alias name
                                HtmlEncodingAttribute htmlAtt = (HtmlEncodingAttribute)attr;
                                if (p.PropertyType == typeof(string))
                                {
                                    switch (htmlAtt.EncodingOption)
                                    {
                                        case HtmlEncoding.AllowSafeHtml:
                                            try
                                            {
                                                var value = p.GetValue(sourceObj, null);
                                                string safeHtml = Sanitizer.GetSafeHtmlFragment((string)value);
                                                _setValue(sourceObj, p, safeHtml);
                                            }
                                            catch (Exception ex){System.Diagnostics.Debug.WriteLine(ex.ToString());}
                                            break;
                                        case HtmlEncoding.AllowUnsafeHtml:
                                            break;
                                        case HtmlEncoding.EncodeHtml:
                                            try
                                            {
                                                var value = p.GetValue(sourceObj, null);
#if CLR_V4
                                                value = Microsoft.Security.Application.Encoder.HtmlEncode((string)value);
#else
                                                value = Microsoft.Security.Application.AntiXss.HtmlEncode((string)value);
#endif
                                                _setValue(sourceObj, p, value);
                                            }
                                            catch (Exception ex)
                                            { System.Diagnostics.Debug.WriteLine(ex.ToString());}
                                            break;
                                        case HtmlEncoding.StripHtml:
                                            try
                                            {
                                                var value = p.GetValue(sourceObj, null);
                                                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                                doc.LoadHtml((string)value);
                                                if (doc == null) break;
                                                string output = "";
                                                foreach (var node in doc.DocumentNode.ChildNodes)
                                                {
                                                    output += node.InnerText;
                                                }
                                                _setValue(sourceObj, p, output);
                                            }
                                            catch (Exception ex)
                                            {System.Diagnostics.Debug.WriteLine(ex.ToString());}
                                            break;
                                    }
                                }
                                //skip to next property
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { System.Diagnostics.Debug.WriteLine(ex.ToString()); }

        }

        static private void _setValue(Object obj, PropertyInfo prop, Object value)
        {
            string propTypeName = prop.PropertyType.Name;
            if (prop.PropertyType.IsGenericType) { propTypeName = Nullable.GetUnderlyingType(prop.PropertyType).Name; }
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
                default: prop.SetValue(obj, value, null); break;
            }
        }
    }
}
