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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FFLib.Data
{
    public interface IDBConnection : System.Data.IDbConnection
    {
        DBProviders.IDBProvider dbProvider { get; }
        int CommandTimeout { get; set; }
        bool InTrx { get; }
        DBTransaction DBTransaction { get; }
        new DBTransaction BeginTransaction();
        void Commit();
        void Rollback();
    }
}
