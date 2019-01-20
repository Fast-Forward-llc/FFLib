/*******************************************************
 * Project: FFLib V1.0
 * Title: LoggerService.cs
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
    public static class LoggerService
    {
        static ILogger _logger;
        static IDiResolver _resolver;
        /// <summary>
        /// This overload initializes the LoggerService with an instance of ILogger by passing Dependancy resolution.
        /// This function cannot be called in combination with the IResolver overload.
        /// </summary>
        /// <param name="logger"></param>
        public static void InitLoggerService(ILogger logger)
        {
            if (_logger != null) throw new ArgumentException("Logger is already Initialized");
            if (_resolver != null) throw new ArgumentException("Resolver has already been intitalized, cannot be re-initialize with a logger instance");
            _logger = logger;
        }
        /// <summary>
        /// Initializes the LoggerService with an instance of IResolver so that ILoggers instances can be obtained via Dependancy Resolution.
        /// This function cannot be called in combination with the IResolver overload.
        /// </summary>
        /// <param name="resolver"></param>
        public static void InitLoggerService(IDiResolver resolver)
        {
            if (_resolver != null) throw new ArgumentException("Resolver is already Initialized");
            if (_logger != null) throw new ArgumentException("Logger has already been intitalized, cannot be re-initialize with a Resolver instance");
            _resolver = resolver;
        }

        /// <summary>
        /// Returns the current initialization state of the logger
        /// </summary>
        /// <returns></returns>
        public static bool IsInitialized(){if (_logger !=null || _resolver != null) return true; else return false;}

        /// <summary>
        /// Get an ILogger Instance by using initilized ILogger or by using Resolver
        /// </summary>
        /// <returns>ILogger</returns>
        public static ILogger GetLogger()
        {
            if (_logger == null && _resolver == null) throw new ArgumentNullException("LoggerService is not initialized with a logger instance or a resolver. Use InitLoggerService() method first.");
            if (_logger != null) return _logger;
            return _resolver.Resolve<ILogger>();

        }

    }
}
