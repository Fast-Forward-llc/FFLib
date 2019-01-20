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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlClient = System.Data.SqlClient;

namespace FFLib.Data.DBProviders
{
    public static class SqlParameterCollectionExtension
    {
        public static void AddRange(this SqlClient.SqlParameterCollection self, IEnumerable<SqlParameter> sqlParameters){
            foreach (var p in sqlParameters)
                self.AddWithValue(p.Name, p.Value);
        }

        public static void AddRange(this SqlClient.SqlParameterCollection self, SqlParameter[] sqlParameters)
        {
            foreach (var p in sqlParameters)
                self.AddWithValue(p.Name, p.Value);
        }
    }

    public static class SPC {
        public static void AddRange(SqlClient.SqlParameterCollection self, SqlParameter[] sqlParameters)
        {
            SqlParameterCollectionExtension.AddRange(self, sqlParameters);
        }
    }

    public class SqlServerProvider : IDBProvider 
    {
        public SqlServerProvider() : base() { this.CommandTimeout = 30; }

        public IDbConnection CreateConnection(string connectionString){
            return new SqlClient.SqlConnection(connectionString);
        }

        public IDbCommand CreateCommand(IDbConnection Connection, string CmdText)
        {
            return (IDbCommand)new SqlClient.SqlCommand(CmdText, (SqlClient.SqlConnection)Connection);
        }

        public int CommandTimeout { get; set; }

        public IDataReader ExecuteReader(IDBConnection conn, string sqlText, dynamic sqlParams)
        {
            
            return this.ExecuteReader(conn, sqlText, SqlParameter.FromDynamic(sqlParams));
        }

        public IDataReader ExecuteReader(IDBConnection conn, string sqlText, SqlParameter[] sqlParams)
        {
            IDbCommand sqlCmd = conn.CreateCommand();
            sqlCmd.CommandText = sqlText;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = this.CommandTimeout;

            if (sqlParams != null && sqlParams.Length > 0) SPC.AddRange((SqlClient.SqlParameterCollection)sqlCmd.Parameters, sqlParams);

            if (conn.State == ConnectionState.Closed) conn.Open();
            #if (SQLDebug) 
                System.Diagnostics.Debug.WriteLine(sqlCmd.CommandText); 
            #endif
            return sqlCmd.ExecuteReader();

        }

        public U DBInsert<U>(IDBConnection conn, string sqlText, dynamic sqlParams)
        {
            return this.DBInsert<U>(conn, sqlText, SqlParameter.FromDynamic(sqlParams));
        }

        public U DBInsert<U>(IDBConnection conn, string sqlText, SqlParameter[] sqlParams)
        {
            IDbCommand sqlCmd = conn.CreateCommand();
            sqlCmd.CommandText = sqlText;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = this.CommandTimeout;

            if (sqlParams != null && sqlParams.Length > 0) SPC.AddRange((SqlClient.SqlParameterCollection)sqlCmd.Parameters, sqlParams);

            if (conn.State == ConnectionState.Closed) conn.Open();
            object rv = null; //temporary reference to executeScalar return value
            U result;
            try
            {
            #if (SQLDebug) 
                            System.Diagnostics.Debug.WriteLine(sqlCmd.CommandText); 
            #endif
                rv = sqlCmd.ExecuteScalar();
                if (rv == DBNull.Value) return default(U);
                result = (U)rv;
            } catch (InvalidCastException) {
                throw new InvalidCastException("Cannot Cast DB Type of:" + rv.GetType().ToString() + " to expected type of:" + typeof(U).ToString());
            }
            return result;
        }

        public void DBUpdate(IDBConnection conn, string sqlText, dynamic sqlParams)
        {
            this.DBUpdate(conn, sqlText, SqlParameter.FromDynamic(sqlParams));
        }

        public void DBUpdate(IDBConnection conn, string sqlText, SqlParameter[] sqlParams)
        {
            IDbCommand sqlCmd = conn.CreateCommand();
            sqlCmd.CommandText = sqlText;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = this.CommandTimeout;

            if (sqlParams != null && sqlParams.Length > 0) SPC.AddRange((SqlClient.SqlParameterCollection)sqlCmd.Parameters, sqlParams);

                if (conn.State == ConnectionState.Closed) conn.Open();
                #if (SQLDebug) 
                                System.Diagnostics.Debug.WriteLine(sqlCmd.CommandText); 
                #endif
                object result = sqlCmd.ExecuteNonQuery();
                
        }

        public U ExecuteScalar<U>(IDBConnection conn, string sqlText, dynamic sqlParams)
        {
            return this.ExecuteScalar<U>(conn, sqlText, SqlParameter.FromDynamic(sqlParams));
        }

        public U ExecuteScalar<U>(IDBConnection conn, string sqlText, SqlParameter[] sqlParams)
        {
            IDbCommand sqlCmd = conn.CreateCommand();
            sqlCmd.CommandText = sqlText;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = this.CommandTimeout;

            if (sqlParams != null && sqlParams.Length > 0) SPC.AddRange((SqlClient.SqlParameterCollection)sqlCmd.Parameters, sqlParams);

            if (conn.State == ConnectionState.Closed) conn.Open();
            #if (SQLDebug) 
                            System.Diagnostics.Debug.WriteLine(sqlCmd.CommandText); 
            #endif
            U result = (U)sqlCmd.ExecuteScalar();

            return result;
        }

        public int ExecuteNonQuery(IDBConnection conn, string sqlText, dynamic sqlParams)
        {
            return this.ExecuteNonQuery(conn, sqlText, SqlParameter.FromDynamic(sqlParams));
        }

        public int ExecuteNonQuery(IDBConnection conn, string sqlText, SqlParameter[] sqlParams)
        {
            IDbCommand sqlCmd = conn.CreateCommand();
            sqlCmd.CommandText = sqlText;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandTimeout = this.CommandTimeout;

            if (sqlParams != null && sqlParams.Length > 0) SPC.AddRange((SqlClient.SqlParameterCollection)sqlCmd.Parameters, sqlParams);

            if (conn.State == ConnectionState.Closed) conn.Open();
            #if (SQLDebug) 
                            System.Diagnostics.Debug.WriteLine(sqlCmd.CommandText); 
            #endif
            return sqlCmd.ExecuteNonQuery();

        }
    }
}
