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
using System.Reflection;

namespace FFLib.Data
{

    public interface IDBContext
    {

        IDBConnection GetScope();

        IDBConnection GetScope(string ScopeId);

        bool CreateScope(string ScopeId, IDBConnection Connection);

        IDBConnection RemoveScope(string ScopeId);

        void UpdateTableCache(string tableName, string key, object row);

        U LoadFromTableCache<U>(string tableName, string key) where U : class;

        bool InTableCache(string tableName, string key);

        bool IsDirty<U>(U row);

        DBContext.FieldList DirtyFields<U>(U row);
    }

    /// <summary>
    /// Provides a global database context that can be shared by multiple objects.
    /// DBContext provides basic caching and single object instance per row which aligns with database semantics. DBContext allows DBTable to support
    /// update-changed-fields-only.
    /// </summary>
    public class DBContext:IDBContext
    {
        #region "classes"
        public class TableCache : Dictionary<string, WeakReference> {
            //initial size of 4x expected size gives better performance, so estimate big
            public TableCache() : base(5000, StringComparer.OrdinalIgnoreCase) { }
        }

        public class FieldList : Dictionary<string, MemberInfo> {
            //initial size of 4x expected size gives better performance, so estimate big
            public FieldList() : base(100) { }
        }
        #endregion

        DBConnection _conn;
        Dictionary<string, TableCache> _cache = new Dictionary<string, TableCache>(500, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, IDBConnection> _dbScopes = new Dictionary<string, IDBConnection>(100, StringComparer.OrdinalIgnoreCase);

        public DBContext(IDBConnection DefaultScopeConnection) :base(){
            this.BaseCreateScope(string.Empty, DefaultScopeConnection);
        }

        public virtual IDBConnection GetScope()
        {
            return this.GetScope(null);
        }

        public virtual IDBConnection GetScope(string ScopeId) {
            if (IsNullOrWhiteSpace(ScopeId)) ScopeId = string.Empty; //default scope
            if (_dbScopes.ContainsKey(ScopeId.Trim())) return _dbScopes[ScopeId];
            return null;
        }

        public virtual bool CreateScope(string ScopeId, IDBConnection Connection)
        {
            return BaseCreateScope(ScopeId, Connection);
        }

        protected bool BaseCreateScope(string ScopeId, IDBConnection Connection) {
            if (Connection == null) throw new ArgumentNullException("Connection cannot be null.");
            if (IsNullOrWhiteSpace(ScopeId)) ScopeId = string.Empty; //default scope
            if (!_dbScopes.ContainsKey(ScopeId.Trim()))
            {
                _dbScopes.Add(ScopeId, Connection);
                return true;
            }
            else
            {
                if (Connection == _dbScopes[ScopeId]) return true; //scope already exists
                else return false;
            }
        }

        public virtual IDBConnection RemoveScope(string ScopeId) {
            if (IsNullOrWhiteSpace(ScopeId)) ScopeId = string.Empty; //default scope
            IDBConnection conn = GetScope(ScopeId);
            if (conn != null) _dbScopes.Remove(ScopeId.Trim());
            return conn;
        }

        public virtual void UpdateTableCache(string tableName, string key, object row)
        {
            if (!_cache.ContainsKey(tableName)) _cache.Add(tableName, new TableCache());
            if (!_cache[tableName].ContainsKey(key)) _cache[tableName].Add(key, new WeakReference(row));
            else _cache[tableName][key].Target = row;

        }

        public virtual U LoadFromTableCache<U>(string tableName, string key) where U : class
        {
            if (!_cache.ContainsKey(tableName)) return null;
            if (!_cache[tableName].ContainsKey(key)) return null;
            if (!_cache[tableName][key].IsAlive || _cache[tableName][key].Target == null)
            {
                _cache[tableName][key].Target = null;
                _cache[tableName].Remove(key);
                return null;
            }
            return (U)_cache[tableName][key].Target;
        }

        public virtual bool InTableCache(string tableName, string key) 
        {
            if (!_cache.ContainsKey(tableName)) return false;
            if (!_cache[tableName].ContainsKey(key)) return false;
            return _cache[tableName][key].IsAlive && _cache[tableName][key].Target != null;
        }

        public virtual bool IsDirty<U>(U row)
        {
            //todo: implement DBContext.IsDirty()
            return true;
        }

        public virtual FieldList DirtyFields<U>(U row)
        {
            //todo: implement DBContext.DirtyFields()
            return null;
        }

        static private bool IsNullOrWhiteSpace(string str)
        {
#if CLR_V2
#if CLR_V4
#error You can't define CLR_V2 and CLR_V4 at the same time
#endif
            return @string.IsNullOrWhiteSpace(str);
#elif CLR_V4    
                return string.IsNullOrWhiteSpace(str);
#else
#error Define either CLR_V2 or CLR_V4 to compile
#endif
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!isDisposed)
            {
                if (disposing)
                {
                if (_conn != null) { _conn.Dispose(); _conn = null; }
                //dispose all connections in this context
                foreach (var k in _dbScopes.Keys)
                { _dbScopes[k].Dispose(); _dbScopes[k] = null; }
                //nullify cache objects and release cache cells
                foreach (var k in _cache.Keys) 
                    foreach(var k2 in _cache[k].Keys)
                    { _cache[k][k2] = null; _cache[k].Remove(k2); }
                }
            }
            // Code to dispose the unmanaged resources
            // held by the class
            _conn = null;
            isDisposed = true;
            
        }
        ~DBContext()
        {
            Dispose (false);
        }
    }
}
