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
using SqlClient = System.Data.SqlClient;

namespace FFLib.Data.DBProviders
{
    public interface IDBProvider
    {
        int CommandTimeout { get; set; }
        IDbConnection CreateConnection(string connectionString);
        IDbCommand CreateCommand(IDbConnection Connection, string CmdText);
        U DBInsert<U>(IDBConnection conn, string sqlText, SqlParameter[] sqlParams);
        U DBInsert<U>(IDBConnection conn, string sqlText, dynamic sqlParams);
        void DBUpdate(IDBConnection conn, string sqlText, SqlParameter[] sqlParams);
        void DBUpdate(IDBConnection conn, string sqlText, dynamic sqlParams);
        int ExecuteNonQuery(IDBConnection conn, string sqlText, SqlParameter[] sqlParams);
        int ExecuteNonQuery(IDBConnection conn, string sqlText, dynamic sqlParams);
        U ExecuteScalar<U>(IDBConnection conn, string sqlText, SqlParameter[] sqlParams);
        U ExecuteScalar<U>(IDBConnection conn, string sqlText, dynamic sqlParams);
        IDataReader ExecuteReader(IDBConnection conn, string sqlText, SqlParameter[] sqlParams);
        IDataReader ExecuteReader(IDBConnection conn, string sqlText, dynamic sqlParams);
    }

}
