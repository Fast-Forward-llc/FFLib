/*******************************************************
 * Project: FFLib V1.0
 * Title: ILogger.cs
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

namespace FFLib.Logging
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }

        void Debug(string message);
        //void Debug(string format, params object[] args);
        //void Debug(Exception exception, string format, params object[] args);
        void Error(string message);
       // void Error(string format, params object[] args);
        //void Error(Exception exception, string format, params object[] args);
        void Fatal(string message);
        //void Fatal(string format, params object[] args);
        //void Fatal(Exception exception, string format, params object[] args);
        void Info(string message);
       // void Info(string format, params object[] args);
        //void Info(Exception exception, string format, params object[] args);
        void Trace(string message);
        //void Trace(string format, params object[] args);
        //void Trace(Exception exception, string format, params object[] args);
        void Warn(string message);
        //void Warn(string format, params object[] args);
        //void Warn(Exception exception, string format, params object[] args);

        void Log(int logLevel,string message);
        //void Log(int logLevel, string format, params object[] args);
       // void Log(int logLevel, Exception exception, string format, params object[] args);
    }
}
