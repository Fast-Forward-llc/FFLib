/*******************************************************
 * Project: FFLib V1.0
 * Title: DBConnection.cs
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
//using System.Transactions;
using FFLib.Data.DBProviders;

namespace FFLib.Data
{

    public class DBConnection : IDBConnection
    {
        System.Data.IDbConnection _conn;
        FFLib.Data.DBProviders.IDBProvider _dbProvider;
        DBTransaction _trx;
        int _trxCnt = 0;


        public DBConnection(IDBProvider dbProvider, IDBConnectionString ConnectionString)
        {
            this.CommandTimeout = 30;
            this._dbProvider = dbProvider;
            _conn = (System.Data.IDbConnection)_dbProvider.CreateConnection(ConnectionString.GetValue());
        }

        public DBConnection(IDBProvider dbProvider, string ConnectionString)
        {
            this.CommandTimeout = 30;
            this._dbProvider = dbProvider;
            _conn = (System.Data.IDbConnection)_dbProvider.CreateConnection(ConnectionString);
        }

        //internal System.Data.SqlClient.SqlConnection Connection { get { return _conn; } }
        public DBProviders.IDBProvider dbProvider { get { return _dbProvider; } }

        public void Open()
        {
            if (_conn.State == System.Data.ConnectionState.Open) return;
            _conn.Open();
            //if (System.Transactions.Transaction.Current != null) _trxCnt++;
        }

        public void Close()
        {
            if (_conn.State == System.Data.ConnectionState.Closed) return;
            _conn.Close();
        }

        public int CommandTimeout { get; set; }

        public string ConnectionString
        {
            get { return _conn.ConnectionString; }
        }

        public System.Data.ConnectionState State
        {
            get { return _conn.State; }
        }

        public bool InTrx
        {
            get { return _trxCnt > 0; }
        }

        public DBTransaction DBTransaction
        {
            get { return _trx; }
        }
        public DBTransaction BeginTransaction()
        {
#if (SQLDebug)
                System.Diagnostics.Debug.Write("Begin Transaction : "); 
#endif

            if (_conn.State == System.Data.ConnectionState.Closed) _conn.Open();
            if (_trx == null) _trx = new DBTransaction(_conn.BeginTransaction(), this);
            _trxCnt++;
#if (SQLDebug)
                  System.Diagnostics.Debug.WriteLine(_trx.GetHashCode());
#endif

            return _trx;
        }

        System.Data.IDbTransaction System.Data.IDbConnection.BeginTransaction()
        {
            return this.BeginTransaction();
        }

        System.Data.IDbTransaction System.Data.IDbConnection.BeginTransaction(System.Data.IsolationLevel level)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
#if (SQLDebug)
                 System.Diagnostics.Debug.Write("Commit Transaction : ");
#endif

            if (_trx == null) { _trxCnt = 0; return; }
            if (_trxCnt == 1) _trx.Transaction.Commit();
#if (SQLDebug)
                 System.Diagnostics.Debug.Write(_trx.GetHashCode());
#endif

            if (_trxCnt > 0) _trxCnt--;
            if (_trxCnt < 1) _trx = null;
        }

        public void Rollback()
        {
#if (SQLDebug)
                 System.Diagnostics.Debug.Write("Commit Transaction : ");
#endif
            if (_trx == null) { _trxCnt = 0; return; }
            _trx.Transaction.Rollback();
#if (SQLDebug)
                 System.Diagnostics.Debug.Write(_trx.GetHashCode());
#endif
            _trxCnt = 0;
            _trx = null;
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        string System.Data.IDbConnection.ConnectionString
        {
            get
            {
                return _conn.ConnectionString;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int ConnectionTimeout
        {
            get { return _conn.ConnectionTimeout; }
        }

        public System.Data.IDbCommand CreateCommand()
        {
            return this.CreateCommand(null);
        }

        public System.Data.IDbCommand CreateCommand(string CmdText)
        {
            System.Data.IDbCommand sqlCmd = _dbProvider.CreateCommand(_conn, CmdText);
            sqlCmd.Connection = _conn;
            sqlCmd.CommandTimeout = this.CommandTimeout;
            if (InTrx) sqlCmd.Transaction = (System.Data.SqlClient.SqlTransaction)_trx.Transaction;
            return sqlCmd;
        }

        public string Database
        {
            get { throw new NotImplementedException(); }
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    _trxCnt = 0;
                    if (_trx != null) { _trx.Dispose(); _trx = null; }
                    if (_conn != null) { _conn.Dispose(); _conn = null; }
                }
            }
            // Code to dispose the unmanaged resources
            // held by the class
            _trx = null;
            _conn = null;
            isDisposed = true;

        }
        ~DBConnection()
        {
            Dispose(false);
        }
    }


    public class DBTransaction : System.Data.IDbTransaction
    {
        System.Data.IDbTransaction _trx;
        DBConnection _conn;

        internal DBTransaction(System.Data.IDbTransaction Transaction, DBConnection Connection)
        {
            _trx = Transaction;
            _conn = Connection;
        }

        internal System.Data.IDbTransaction Transaction { get { return _trx; } }

        public void Commit()
        {
            _conn.Commit();
        }

        public System.Data.IDbConnection Connection
        {
            get { return _conn; }
        }

        public System.Data.IsolationLevel IsolationLevel
        {
            get { throw new NotImplementedException(); }
        }

        public void Rollback()
        {
            _conn.Rollback();
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (_trx != null)
                        if (_conn == null) { _trx.Dispose(); _trx = null; }
                        else
                        if (!_conn.InTrx) { _trx.Dispose(); _trx = null; }
                }
            }
        }
        ~DBTransaction()
        {
            Dispose(false);
        }
    }
}
