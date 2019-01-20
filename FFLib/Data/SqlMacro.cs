/*******************************************************
 * Project: FFLib V1.0
 * Title: SqlMacro.cs
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
using FFLib.Extensions;

namespace FFLib.Data
{

    public class SqlMacro
    {
        public SqlMacro() { }
        public SqlMacro(string TokenPrefix)
        {
            _tokenprefix = TokenPrefix;
        }

        public SqlMacro(MacroTypes mtype, string Name, object Value)
        {
            this.MacroType = mtype;
            this.Name = Name;
            this.Value = Value;
        }

        public enum MacroTypes {
            Literal = 0,
            Identifier = 1,
            Keyword = 3
        }

        protected object _value;

        private string _tokenprefix = @"##_";

        public virtual MacroTypes MacroType { get; set; }

        public virtual string Name { get; set; }
        /// <summary>
        /// This is the literal value that will be searched for and substituted with the macro's value
        /// </summary>
        public virtual string Token { get { return  _tokenprefix + Name; } }

        public virtual object Value { get { return _value; } set { _value = value; } }

        public virtual bool IsNumeric() { return false; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    public class SqlMacro<T> : SqlMacro
    {
        public SqlMacro():base() { }
        public SqlMacro(string TokenPrefix) : base(TokenPrefix) { }
        public SqlMacro(MacroTypes mtype, string Name, T Value) : base(mtype, Name, Value) { }

        public new virtual T Value { get { return (T)_value; } set { _value = value; } }

        public override bool IsNumeric() {
            if (Value is int || Value is long || Value is short || Value is bool) return true;
            if (Value is decimal || Value is double || Value is Single) return true;
            if (Value is uint || Value is ulong || Value is ushort ) return true;
            return false; 
        }

        public override string ToString() 
        {
            if (Value is bool) { return Convert.ToBoolean(Value) ? "1" : "0"; }

            if (this.MacroType == MacroTypes.Identifier) return "[" + string.Join("].[", Value.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)) + "]";
            if (this.MacroType == MacroTypes.Literal)
                if (typeof(T).IsArray)
                {
                    List<string> results = new List<string>();
                    Array items = Value as Array;
                    foreach (T i in items) { results.Add(this.IsNumeric() ? i.ToString() : i.ToString().SqlQuote()); }
                    return string.Join(",", results.ToArray());
                }
                else
                    return (this.IsNumeric()) ? Value.ToString() : Value.ToString().SqlQuote();
            
            if (this.MacroType == MacroTypes.Keyword) return Value.ToString();
            return Value.ToString();
            
        }
    }
}
