/*******************************************************
 * Project: FFLib V1.0
 * Title: ORMInterfaces.cs
 * Author: Phillip Bird of Fast Forward,LLC
 * Copyright © 2012 Fast Forward, LLC. 
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * Use of any component of FFLib requires acceptance and adhearance 
 * to the terms of either the MIT License or the GNU General Public License (GPL) Version 2 exclusively.
 * Notification of license selection is not required and will be infered based on applicability.
 * Contributions to FFLib requires a contributor grant on file with Fast Forward, LLC.
********************************************************/
//disable Missing XML documentation warning
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib
{
    public class FuncResult<T>
    {
        public virtual bool Success { get; set; }
        public virtual T Value { get; set; }
        public virtual int Code { get; set; }
        public virtual string Msg {get; set;}

        public FuncResult() : base() { this.Success = false; }
        public FuncResult(T value): this(true,value,null)
        {}
        public FuncResult(bool success, T value, string msg) : this(){
            this.Success = success;
            this.Value = value;
            this.Msg = msg;
        }

        public FuncResult<T> SetStatus(bool success, T value)
        {
            return this.SetStatus(success,value,0,null);
        }
        public FuncResult<T> SetStatus(bool success, T value, int code)
        {
            return this.SetStatus(success, value, code, null);
        }
        public FuncResult<T> SetStatus(bool success, T value, string msg)
        {
            return this.SetStatus(success, value, 0, msg);
        }
        public FuncResult<T> SetStatus(bool success, T value, int code, string msg)
        {
            this.Success = success;
            this.Value = value;
            this.Msg = msg;
            return this;
        }
    }
}
