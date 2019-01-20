/*******************************************************
 * Project: FFLib V1.0
 * Title: DBTable.cs
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
using System.Data;
using System.Text.RegularExpressions;
using FFLib.Data.Attributes;
using FFLib.Data.DBProviders;
using FFLib.Extensions;
using System.Diagnostics;

namespace FFLib.Data
{
    public interface IDBTable<T> where T : class, new()
    {
        T Load(int ID);
        T[] Load(string sql,SqlParameter[] SqlParams);
        T[] Load(string SqlText, SqlMacro[] SqlMacros);
        T[] Load(string SqlText, SqlMacro[] SqlMacros, SqlParameter[] SqlParams);
        T LoadOne(string SqlText);
        T LoadOne(string SqlText, SqlParameter[] paramList);
        Dictionary<string, T> LoadAssoc(string sql, SqlParameter[] SqlParams, string key);
        Dictionary<string, T> LoadAssoc(string sql, SqlMacro[] SqlMacros, SqlParameter[] SqlParams, string keyfield);
        Dictionary<string, List<T>> LoadAssocList(string sql, SqlParameter[] SqlParams, string keyfield);
        int Execute(string SqlText, SqlParameter[] SqlParams);
        int Execute(string SqlText, SqlMacro[] SQLMacros);
        int Execute(string SqlText, SqlMacro[] SQLMacros, SqlParameter[] SqlParams);
        object ExecuteScalar(string SqlText);
        object ExecuteScalar(string SqlText, SqlMacro[] SQLMacros);
        object ExecuteScalar(string SqlText, SqlMacro[] SQLMacros, SqlParameter[] SqlParams);
        string ParseSql(string SqlText);
        string ParseSql(string SqlText, SqlMacro[] SQLMacros);
        void Save(T obj);
    }

    /// <summary>
    /// Field Definition info for a DTO field.
    /// </summary>
    public class FieldDef
    {
        MemberInfo _memberInfo;
        string _mappingName;
        string _fieldName;
        DBType? _castAsType;

        public FieldDef(MemberInfo memberInfo, string mappingName, string fieldName, DBType? castAsType) : base() {
            _memberInfo = memberInfo;
            _mappingName = mappingName;
            _fieldName = fieldName;
            _castAsType = castAsType;
        }

        public MemberInfo MemberInfo { get {return _memberInfo; } }

        public string MappingName { get { return _mappingName; } }

        public string FieldName { get { return _fieldName; } }

        public DBType? CastAsType { get { return _castAsType; } }
    }

    public class DBTable<T> : IDBTable<T> where T : class, new()
    {
        protected IDBConnection _conn = null;
        protected IDBProvider _dbProvider = null;
        protected IDBContext _dbContext = null;
        protected string _pk = null;
        protected MemberInfo _pk_memberinfo = null;
        protected string _tableName = null;
        protected int _commandTimeout = 0;
        protected FieldDef[] _fieldTypeCache;

        //static Regex subTableNameRegex = new Regex("#__TableName", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //static Regex subPKRegex = new Regex("#__PK", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public int CommandTimeout { 
            get{
                if (_commandTimeout > 0) return _commandTimeout;
                return _conn.CommandTimeout;
            }
            set { _commandTimeout = value; } 
        }

        public DBTable(IDBConnection Conn)
            : this(Conn, null)
        {  }

        public DBTable(IDBConnection Conn, string tableName)
            : base()
        {
            //CommandTimeout = 30;
            _conn = Conn;
            DBTableHelper.TableDef tableDef = DBTableHelper.GetTableDef<T>(tableName);
            _pk = tableDef.PK;
            _pk_memberinfo = tableDef.PK_MemberInfo;
            _tableName = tableDef.TableName;
            _dbProvider = _conn.dbProvider;
            _dbProvider.CommandTimeout = CommandTimeout;
            _fieldTypeCache = tableDef.Fields;

        }

        public DBTable(IDBContext dbContext, IDBConnection Conn)
            : this(Conn, null){}

        public DBTable(IDBContext dbContext, IDBConnection Conn, string tableName)
            : this(Conn, tableName)
        {
            _dbContext = dbContext;
        }

        public IDBConnection DBConnection { get { return _conn; } }

        /// <summary>
        /// Loads a single object by it's integer Id value. if not found an new empty object will be returned.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>object instance of record or new empty instance (using parameterless contructor). Never returns null.</returns>
        public virtual T Load(int ID)
        {
            string sqlText = this.ParseSql("SELECT * FROM #__TableName WHERE #__PK = @id");
            SqlParameter id = new SqlParameter("@id", ID);
            T[] rows = this.Load(sqlText, new SqlParameter[] { id });
            if (rows == null || rows.Length == 0) return new T();
            return rows[0];
        }

        public virtual T LoadOne(string SqlText)
        {
            return this.LoadOne(SqlText, null);
        }

        public virtual T LoadOne(string SqlText, SqlParameter[] paramList)
        {
            T[] results;
            results = this.Load(SqlText, null, paramList);
            if (results == null || results.Length == 0) return null;
            return results[0];
        }

        public virtual T LoadOne(SqlQuery SqlText)
        {
            return this.LoadOne(SqlText, SqlText.SqlParams.ToArray());
        }

        public virtual T LoadOne(SqlQuery SqlText, SqlParameter[] paramList)
        {
            T[] results;
            if (paramList != null) SqlText.SqlParams.AddRange(paramList);
            results = this.Load(SqlText.ToString(), null, SqlText.SqlParams.ToArray());
            if (results == null || results.Length == 0) return null;
            return results[0];
        }

        public virtual T LoadOne(string SqlText, dynamic paramList)
        {
            T[] results;
            results = this.Load(SqlText, null, paramList);
            if (results == null || results.Length == 0) return null;
            return results[0];
        }

        public virtual T LoadOne(SqlQuery SqlText, dynamic paramList)
        {
            T[] results;
            var sp = SqlParameter.FromDynamic(paramList);
            SqlText.SqlParams.AddRange(sp);
            results = this.Load(SqlText);
            if (results == null || results.Length == 0) return null;
            return results[0];
        }

        public virtual dynamic LoadOneDynamic(string SqlText, dynamic SqlParams)
        {
            System.Data.IDataReader reader = null;
            try
            {
                dynamic[] r = this.LoadDynamic(SqlText,SqlParams);
                if (r == null || r.Length == 0) return null;
                return r[0];
            }
            finally
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); reader.Dispose(); }
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        public virtual T[] Load(string SqlText)
        {
            return this.Load(SqlText, null, null);
        }

        public virtual T[] Load(string SqlText, dynamic SqlParam)
        {
            return this.Load(SqlText, null, SqlParam);
        }

        public virtual T[] Load(string SqlText, SqlParameter SqlParam)
        {
            return this.Load(SqlText, null, new SqlParameter[]{SqlParam});
        }

        public virtual T[] Load(string SqlText, SqlParameter[] SqlParams)
        {
            return this.Load(SqlText, null, SqlParams);
        }
        /********************************/
        public virtual T[] Load(SqlQuery SqlText)
        {
            return this.Load(SqlText.ToString(), SqlText.SqlParams.ToArray());
        }

        public virtual T[] Load(SqlQuery SqlText, dynamic SqlParam)
        {
            var sp = SqlParameter.FromDynamic(SqlParam);
            SqlText.SqlParams.AddRange(sp);
            return this.Load(SqlText);
        }

        public virtual T[] Load(SqlQuery SqlText, SqlParameter[] SqlParams)
        {
            if (SqlParams != null) SqlText.SqlParams.AddRange(SqlParams);
            return this.Load(SqlText);
        }
        /****************************/

        public virtual T[] Load(string SqlText, SqlMacro[] SqlMacros) {
            return this.Load(SqlText, SqlMacros, null);
        }

        public virtual T[] Load(string SqlText, SqlMacro[] SqlMacros, SqlParameter[] SqlParams) {
            System.Data.IDataReader reader = null;
           // Sql.SqlCommand sqlCmd = new Sql.SqlCommand(this.ParseSql(SqlText,SqlMacros), _conn.Connection);
           // if (SqlParams != null) sqlCmd.Parameters.AddRange(SqlParams);
            try{
                _conn.Open();
                //long t = DateTime.Now.Ticks;
                //Debug.WriteLine("DB Load Start:" + (DateTime.Now.Ticks - t));
                _dbProvider.CommandTimeout = this.CommandTimeout;
                reader = _dbProvider.ExecuteReader(_conn, this.ParseSql(SqlText, SqlMacros), SqlParams );
                //Debug.WriteLine("DB Reader Done:" + (DateTime.Now.Ticks - t));
                T[] r = this.Bind(reader);
                //Debug.WriteLine("Bind done:" + (DateTime.Now.Ticks - t));
                return r;
            }
            finally {
                if (reader != null && !reader.IsClosed ) {reader.Close(); reader.Dispose();}
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        public virtual T[] Load(string SqlText, SqlMacro[] SqlMacros, dynamic SqlParams)
        {
            System.Data.IDataReader reader = null;
            try
            {
                _conn.Open();
                _dbProvider.CommandTimeout = this.CommandTimeout;
                reader = _dbProvider.ExecuteReader(_conn, this.ParseSql(SqlText, SqlMacros), SqlParams);
                T[] r = this.Bind(reader);

                return r;
            }
            finally
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); reader.Dispose(); }
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        public virtual dynamic[] LoadDynamic(string SqlText, dynamic SqlParams)
        {
            return this.LoadDynamic(SqlText, null, SqlParams);
        }

        public virtual dynamic[] LoadDynamic(string SqlText, SqlMacro[] SqlMacros, dynamic SqlParams)
        {
            System.Data.IDataReader reader = null;
            try
            {
                _conn.Open();
                _dbProvider.CommandTimeout = this.CommandTimeout;
                reader = _dbProvider.ExecuteReader(_conn, this.ParseSql(SqlText, SqlMacros), SqlParams);
                dynamic[] r = this.BindDynamic(reader);

                return r;
            }
            finally
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); reader.Dispose(); }
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        /// <summary>
        /// Loads an array of objects whose Primary keys are in idList. 
        /// </summary>
        /// <typeparam name="priKeyType">Primary key data type</typeparam>
        /// <param name="idList">Array of Id's to load</param>
        /// <returns></returns>
        public virtual T[] Load<priKeyType>(priKeyType[] idList) 
        {
            SqlMacro pk = new SqlMacro(SqlMacro.MacroTypes.Keyword,"pk",string.Join(",",idList.EscapeSqlList(true)));
            string sql = "SELECT * FROM #__TableName WHERE #__PK in ("+pk.Token+")";
            return this.Load(sql, new SqlMacro[]{pk}, null);
        }

        /// <summary>
        /// Loads an array of objects whose Primary keys are in idList. 
        /// </summary>
        /// <param name="idList">Array of Id's to load</param>
        /// <returns></returns>
        public virtual T[] Load(int[] idList)
        {
            SqlMacro pk = new SqlMacro(SqlMacro.MacroTypes.Keyword, "pk", string.Join(",", idList));
            string sql = "SELECT * FROM #__TableName WHERE #__PK in (" + pk.Token + ")";
            return this.Load(sql, new SqlMacro[] { pk }, null);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="SqlParam"></param>
        /// <param name="keyfield"></param>
        /// <returns></returns>
        public virtual Dictionary<string, T> LoadAssoc(string sql, dynamic SqlParam, string keyfield)
        {
            return this.LoadAssoc(sql, null, SqlParam, keyfield);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="SqlParam"></param>
        /// <param name="keyfield"></param>
        /// <returns></returns>
        public virtual Dictionary<string, T> LoadAssoc(SqlQuery sql, dynamic SqlParam, string keyfield)
        {
            var sp = SqlParameter.FromDynamic(SqlParam);
            sql.SqlParams.AddRange(sp);
            return this.LoadAssoc(sql, keyfield);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        public virtual Dictionary<string, T> LoadAssoc(string sql, SqlParameter[] SqlParams, string keyfield)
        {
            return LoadAssoc(sql, null, SqlParams, keyfield);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        public virtual Dictionary<string, T> LoadAssoc(SqlQuery sql, string keyfield)
        {
            return LoadAssoc(sql.ToString(), null, sql.SqlParams.ToArray(), keyfield);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        public virtual Dictionary<string, T> LoadAssoc(string sql, SqlMacro[] SqlMacros, dynamic SqlParams, string keyfield)
        {
            return this.LoadAssoc(sql, SqlMacros, SqlParameter.FromDynamic(SqlParams), keyfield);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        public virtual Dictionary<string, T> LoadAssoc(string sql, SqlMacro[] SqlMacros, SqlParameter[] SqlParams, string keyfield)
        {
            Dictionary<string, T> results = new Dictionary<string, T>();
            if (string.IsNullOrEmpty(keyfield) || keyfield.Trim() == string.Empty) return results;
            if (string.IsNullOrEmpty(sql) || sql.Trim() == string.Empty) return results;

            T[] rows = this.Load(sql, SqlMacros, SqlParams);

            foreach (T row in rows)
            {
                MemberInfo[] mi = row.GetType().GetMember(keyfield, MemberTypes.Field | MemberTypes.Property,BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (mi == null || mi.Length == 0) continue;
                switch (mi[0].MemberType ){
                    case MemberTypes.Property:
                            PropertyInfo pi = mi[0] as PropertyInfo;
                            this.AddRowAssoc(results, pi, row);
                        break;
                    case MemberTypes.Field:
                            FieldInfo fi = mi[0] as FieldInfo;
                            this.AddRowAssoc(results, fi, row);
                        break;
                    default: continue;
                }
            }
            return results;
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        public virtual Dictionary<string, T> LoadAssoc(string sql, SqlParameter SqlParam, string keyfield)
        {
            return this.LoadAssoc(sql, null, new SqlParameter[] { SqlParam }, keyfield);
        }

        /// <summary>
        /// Load Associative List.
        /// Loads objects into a dictionary keyed by the KeyField parameter
        /// </summary>
        public virtual Dictionary<string, T> LoadAssoc(string sql, string keyfield)
        {
            return this.LoadAssoc(sql, null, null, keyfield);
        }

        /// <summary>
        /// Load resultset into 2 dimensional array of object where the first dimension = rows and the second dimension = columns
        /// columns are in the order they are returned from the query in the resultset
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="SqlMacros"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public virtual object[][] LoadArrayList(string sql, SqlMacro[] SqlMacros, dynamic SqlParams)
        {
            return this.LoadArrayList(sql, SqlMacros, SqlParameter.FromDynamic(SqlParams));
        }

        /// <summary>
        /// Load resultset into 2 dimensional array of object where the first dimension = rows and the second dimension = columns
        /// columns are in the order they are returned from the query in the resultset
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="SqlMacros"></param>
        /// <param name="SqlParams"></param>
        /// <returns></returns>
        public virtual object[][] LoadArrayList(string sql, SqlMacro[] SqlMacros, SqlParameter[] SqlParams)
        {
            List<object[]> results = new List<object[]>();
            if (string.IsNullOrEmpty(sql) || sql.Trim() == string.Empty) return new object[][] {};
            System.Data.IDataReader reader = null;
            try
            {
                _conn.Open();
                //long t = DateTime.Now.Ticks;
                //Debug.WriteLine("DB Load Start:" + (DateTime.Now.Ticks - t));
                _dbProvider.CommandTimeout = this.CommandTimeout;
                reader = _dbProvider.ExecuteReader(_conn, this.ParseSql(sql, SqlMacros), SqlParams);
                //Debug.WriteLine("DB Reader Done:" + (DateTime.Now.Ticks - t));
                //T[] r = this.Bind(reader);
                while (reader.Read())
                {
                    object[] r = new object[reader.FieldCount];
                    results.Add(r);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object t = reader.GetValue(i);
                        if (t == DBNull.Value)
                        {
                            r[i]=null;
                        }
                        else
                        {
                            r[i]=t;
                        }
                    }
                }
                //Debug.WriteLine("Bind done:" + (DateTime.Now.Ticks - t));
                return results.ToArray();
            }
            finally
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); reader.Dispose(); }
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        public virtual Dictionary<string, List<T>> LoadAssocList(string sql, dynamic SqlParams, string keyfield)
        {
            return LoadAssocList(sql, null, SqlParams, keyfield);
        }

        public virtual Dictionary<string, List<T>> LoadAssocList(string sql, SqlParameter[] SqlParams, string keyfield)
        {
            return LoadAssocList(sql, null, SqlParams, keyfield);
        }

        public virtual Dictionary<string, List<T>> LoadAssocList(SqlQuery sql, dynamic SqlParams, string keyfield)
        {
            var sp = SqlParameter.FromDynamic(SqlParams);
            sql.SqlParams.AddRange(sp);
            return LoadAssocList(sql, keyfield);
        }

        public virtual Dictionary<string, List<T>> LoadAssocList(SqlQuery sql, string keyfield)
        {
            return LoadAssocList(sql.ToString(), null, sql.SqlParams.ToArray(), keyfield);
        }

        public virtual Dictionary<string, List<T>> LoadAssocList(string sql, SqlMacro[] SqlMacros, dynamic SqlParams, string keyfield)
        {
            return this.LoadAssocList(sql, SqlMacros, SqlParameter.FromDynamic(SqlParams), keyfield);
        }

        public virtual Dictionary<string, List<T>> LoadAssocList(string sql, SqlMacro[] SqlMacros, SqlParameter[] SqlParams, string keyfield)
        {
            Dictionary<string, List<T>> results = new Dictionary<string, List<T>>();
            if (string.IsNullOrEmpty(keyfield) || keyfield.Trim() == string.Empty) return results;
            if (string.IsNullOrEmpty(sql) || sql.Trim() == string.Empty) return results;

            T[] rows = this.Load(sql, SqlMacros, SqlParams);
            var m = _fieldTypeCache.Where(x => x.FieldName == keyfield).FirstOrDefault();
            var mi = (m?.MemberInfo);
            if (mi == null) return results;
            foreach (T row in rows)
            {
                switch (mi.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo pi = mi as PropertyInfo;
                        this.AddRowAssocList(results, pi, row);
                        break;
                    case MemberTypes.Field:
                        FieldInfo fi = mi as FieldInfo;
                        this.AddRowAssocList(results, fi, row);
                        break;
                    default: continue;
                }
            }
            return results;
        }

        public virtual R[] LoadScalarArray<R>(int index, string SqlText)
        {
            return this.LoadScalarArray<R>(index, SqlText, null, (SqlParameter[])null);
        }

        public virtual R[] LoadScalarArray<R>(int index, SqlQuery SqlText)
        {
            return this.LoadScalarArray<R>(index, SqlText.ToString(), null, SqlText.SqlParams.ToArray());
        }

        public virtual R[] LoadScalarArray<R>(int index, string SqlText, SqlMacro[] SqlMacros, SqlParameter SqlParam)
        {
            return this.LoadScalarArray<R>(index, SqlText, SqlMacros, new SqlParameter[] { SqlParam });
        }

        public virtual R[] LoadScalarArray<R>(int index, string SqlText, dynamic SqlParams)
        {
            return this.LoadScalarArray<R>(index, SqlText, null , SqlParameter.FromDynamic(SqlParams));
        }

        public virtual R[] LoadScalarArray<R>(int index, SqlQuery SqlText, dynamic SqlParams)
        {
            var sp = SqlParameter.FromDynamic(SqlParams);
            SqlText.SqlParams.AddRange(sp);
            return this.LoadScalarArray<R>(index, SqlText);
        }

        public virtual R[] LoadScalarArray<R>(int index, string SqlText, SqlMacro[] SqlMacros, dynamic SqlParams)
        {
            return this.LoadScalarArray<R>(index, SqlText, SqlMacros, SqlParameter.FromDynamic(SqlParams));
        }

        public virtual R[] LoadScalarArray<R>(int index, string SqlText, SqlMacro[] SqlMacros, SqlParameter[] SqlParams) 
        {
            System.Data.IDataReader reader = null;
            List<R> r = new List<R>();
            // Sql.SqlCommand sqlCmd = new Sql.SqlCommand(this.ParseSql(SqlText,SqlMacros), _conn.Connection);
            // if (SqlParams != null) sqlCmd.Parameters.AddRange(SqlParams);
            try
            {
                _conn.Open();
                //long t = DateTime.Now.Ticks;
                //Debug.WriteLine("DB Load Start:" + (DateTime.Now.Ticks - t));
                _dbProvider.CommandTimeout = this.CommandTimeout;
                reader = _dbProvider.ExecuteReader(_conn, this.ParseSql(SqlText, SqlMacros), SqlParams);
                //Debug.WriteLine("DB Reader Done:" + (DateTime.Now.Ticks - t));
                //T[] r = this.Bind(reader);
                while (reader.Read())
                {
                    object t = reader.GetValue(index);
                    if (t == DBNull.Value && typeof(R).IsValueType)
                    {
                        R q = default(R); //create a min value struct of type R
                        r.Add(q);
                    }
                    else
                    {
                        if (t is DBNull) continue;
                        r.Add((R)t);
                    }
                }
                //Debug.WriteLine("Bind done:" + (DateTime.Now.Ticks - t));
                return r.ToArray();
            }
            finally
            {
                if (reader != null && !reader.IsClosed) { reader.Close(); reader.Dispose(); }
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        protected virtual void AddRowAssoc(Dictionary<string,T> AssocList, PropertyInfo pi, T row){
            object key = pi.GetValue(row,null);
            if (AssocList.ContainsKey(key.ToString())) return;

            AssocList.Add(key.ToString(), row);
        }

        protected virtual void AddRowAssoc(Dictionary<string, T> AssocList, FieldInfo fi, T row)
        {
            object key = fi.GetValue(row);
            if (AssocList.ContainsKey(key.ToString())) return;

            AssocList.Add(key.ToString(), row);
        }

        protected virtual void AddRowAssocList(Dictionary<string, List<T>> AssocList, PropertyInfo pi, T row)
        {
            object key = pi.GetValue(row, null);
            if (AssocList.ContainsKey(key.ToString())) AssocList[key.ToString()].Add(row);
            else
                AssocList.Add(key.ToString(),new List<T>(new T[]{row}));
        }

        protected virtual void AddRowAssocList(Dictionary<string, List<T>> AssocList, FieldInfo fi, T row)
        {
            object key = fi.GetValue(row);
            if (AssocList.ContainsKey(key.ToString())) AssocList[key.ToString()].Add(row);
            else
                AssocList.Add(key.ToString(), new List<T>(new T[] { row }));
        }

        public int Execute(string SqlText)
        {
            return this.Execute(SqlText, null, null);
        }

        public int Execute(SqlQuery SqlText)
        {
            return this.Execute(SqlText.ToString(), null, SqlText.SqlParams.ToArray());
        }

        public int Execute(string SqlText, SqlParameter SqlParam)
        {
            return this.Execute(SqlText, null, new SqlParameter[]{SqlParam});
        }

        public int Execute(string SqlText, dynamic SqlParam)
        {
            return this.Execute(SqlText, null, SqlParam);
        }

        public int Execute(SqlQuery SqlText, dynamic SqlParam)
        {
            var sp = SqlParameter.FromDynamic(SqlParam);
            SqlText.SqlParams.AddRange(sp);
            return this.Execute(SqlText);
        }

        public int Execute(string SqlText, SqlParameter[] SqlParams)
        {
            return this.Execute(SqlText, null, SqlParams);
        }

        public int Execute(string SqlText, SqlMacro[] SQLMacros)
        {
            return this.Execute(SqlText, SQLMacros, null);
        }

        public int Execute(string SqlText, SqlMacro[] SQLMacros, dynamic SqlParams)
        {
            return this.Execute(SqlText, SQLMacros, SqlParameter.FromDynamic(SqlParams));
        }

        /// <summary>
        /// Execute SQL Batch returning the number of rows affected
        /// </summary>
        /// <param name="SqlText">SQL statements to be executed</param>
        /// <param name="SQLMacros">Array of SQLMacros. SQLMacros will be substituted in to the SQL String before execution</param>
        /// <param name="SqlParams">Array of SQL Parameters</param>
        /// <returns>int : number of rows affected. this may be altered by sql statements such as nocount</returns>
        public int Execute(string SqlText, SqlMacro[] SQLMacros, SqlParameter[] SqlParams)
        {
            //Sql.SqlCommand sqlCmd = _conn.CreateCommand(this.ParseSql(SqlText, SQLMacros));
            //if (SqlParams != null) sqlCmd.Parameters.AddRange(SqlParams);
            try
            {
                _conn.Open();
                _dbProvider.CommandTimeout = this.CommandTimeout;
                return _dbProvider.ExecuteNonQuery(_conn, this.ParseSql(SqlText, SQLMacros), SqlParams);
            }
            finally
            {
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        public object ExecuteScalar(string SqlText)
        {
            return this.ExecuteScalar(SqlText, null, null);
        }

        public object ExecuteScalar(string SqlText, SqlParameter[] SqlParams)
        {
            return this.ExecuteScalar(SqlText, null, SqlParams);
        }

        public object ExecuteScalar(string SqlText, dynamic SqlParams)
        {
            return this.ExecuteScalar(SqlText, null, SqlParameter.FromDynamic(SqlParams));
        }

        public object ExecuteScalar(string SqlText, SqlMacro[] SQLMacros)
        {
            return this.ExecuteScalar(SqlText, SQLMacros, null);
        }

        public object ExecuteScalar(string SqlText, SqlMacro[] SQLMacros, dynamic SqlParams)
        {
            return this.ExecuteScalar(SqlText, SQLMacros, SqlParameter.FromDynamic(SqlParams));
        }
        /// <summary>
        /// Execute SQL Batch returning the first value of the first row of the resultset
        /// </summary>
        /// <param name="SqlText">SQL statements to be executed</param>
        /// <param name="SQLMacros">Array of SQLMacros. SQLMacros will be substituted in to the SQL String before execution</param>
        /// <param name="SqlParams">Array of SQL Parameters</param>
        /// <returns>object : the first value of the first row of the resultset</returns>
        public object ExecuteScalar(string SqlText, SqlMacro[] SQLMacros, SqlParameter[] SqlParams)
        {
            //Sql.SqlCommand sqlCmd = _conn.CreateCommand(this.ParseSql(SqlText, SQLMacros));
            //if (SqlParams != null) sqlCmd.Parameters.AddRange(SqlParams);
            try
            {
                _conn.Open();
                _dbProvider.CommandTimeout = this.CommandTimeout;
                return _dbProvider.ExecuteScalar<object>(_conn, this.ParseSql(SqlText, SQLMacros), SqlParams);
            }
            finally
            {
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }
        }

        public U ExecuteScalar<U>(string SqlText, SqlMacro[] SQLMacros, dynamic SqlParams)
        {
            return this.ExecuteScalar<U>(SqlText, SQLMacros, SqlParameter.FromDynamic(SqlParams));
        }

        public U ExecuteScalar<U>(string SqlText, SqlMacro[] SQLMacros, SqlParameter[] SqlParams)
        {
            object o = ExecuteScalar(SqlText, SQLMacros, SqlParams);
            if ( o == null || o == DBNull.Value) return default(U);
            return (U)o; 
        }

        public U ExecuteScalar<U>(string SqlText, dynamic SqlParams)
        {
            return this.ExecuteScalar<U>(SqlText, null, SqlParameter.FromDynamic(SqlParams));
        }

        public U ExecuteScalar<U>(string SqlText, SqlParameter SqlParam)
        {
            var sqlParams = new SqlParameter[] { };
            if (SqlParam != null) sqlParams = new SqlParameter[] { SqlParam };
            return this.ExecuteScalar<U>(SqlText, null, sqlParams);
        }

        public U ExecuteScalar<U>(SqlQuery SqlText, dynamic SqlParams)
        {
            var sp = SqlParameter.FromDynamic(SqlParams);
            SqlText.SqlParams.AddRange(sp);
            return this.ExecuteScalar<U>(SqlText.ToString(), null, SqlText.SqlParams.ToArray());
        }

        public U ExecuteScalar<U>(SqlQuery SqlText)
        {
            return this.ExecuteScalar<U>(SqlText.ToString(), null, SqlText.SqlParams.ToArray());
        }

        protected virtual T[] Bind(IDataReader reader)
        {
            List<T> rows = new List<T>();
            try
            {
            if (reader == null || reader.IsClosed) return rows.ToArray();

                //MemberInfo[] miList = typeof(T).GetMember("*", MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public);
                var miList = this._fieldTypeCache;
                Dictionary<string,int> columnIdx = new Dictionary<string,int>();
            
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnIdx.Add(reader.GetName(i), i);
                }
                while (reader.Read())
                {
                    T dto = new T();
                    foreach (FieldDef f in _fieldTypeCache)
                    {
                        if (!columnIdx.ContainsKey(f.MappingName) && !columnIdx.ContainsKey(f.FieldName)) continue;
                        string mName = columnIdx.ContainsKey(f.MappingName) ? f.MappingName : f.FieldName;
                        switch (f.MemberInfo.MemberType)
                        {
                            case MemberTypes.Field:
                                FieldInfo fi = f.MemberInfo as FieldInfo;
                                DBTable<T>._SetValue(dto, f.MemberInfo, reader.GetValue(columnIdx[mName]) == DBNull.Value ? null : f.CastAsType != null ? this.DBCastAs(f.CastAsType.Value, reader.GetValue(columnIdx[mName])) : reader.GetValue(columnIdx[mName]));
                                break;
                            case MemberTypes.Property:
                                PropertyInfo pi = f.MemberInfo as PropertyInfo;
                                if (pi.CanWrite) DBTable<T>._SetValue(dto, f.MemberInfo, reader.GetValue(columnIdx[mName]) == DBNull.Value ? null : f.CastAsType != null ? this.DBCastAs(f.CastAsType.Value, reader.GetValue(columnIdx[mName])) : reader.GetValue(columnIdx[mName]));
                                break;
                        }
                    }
                    if (dto is ISupportsIsDirty) ((ISupportsIsDirty)dto).InitCleanState();
                    rows.Add(dto);
                    if (_dbContext != null) _dbContext.UpdateTableCache(_tableName, this.GetPKValue(dto).ToString(),dto);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return rows.ToArray();
        }

        protected virtual dynamic[] BindDynamic(IDataReader reader)
        {
            List<dynamic> rows = new List<dynamic>();
            try
            {
                if (reader == null || reader.IsClosed) return rows.ToArray();

                //MemberInfo[] miList = typeof(T).GetMember("*", MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public);
                Dictionary<string, int> columnIdx = new Dictionary<string, int>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnIdx.Add(reader.GetName(i), i);
                }
                while (reader.Read())
                {
                    dynamic dto = new System.Dynamic.ExpandoObject();
                    var expandoDict = dto as IDictionary<string, object>;

                    for (var i = 0; i < reader.FieldCount; i++) 
                    {
                        string fieldname = reader.GetName(i);
                        
                        if (expandoDict.ContainsKey(fieldname))
                            expandoDict[fieldname] = reader[i];
                        else
                            expandoDict.Add(fieldname, reader[i]);
                    }
                    rows.Add(dto);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return rows.ToArray();
        }

        static private void _SetValue(Object obj, MemberInfo m, Object value)
        {
            //string propTypeName = null;
            switch (m.MemberType)
            {
                case MemberTypes.Property:
                    {
                        PropertyInfo prop = m as PropertyInfo;
                        //propTypeName = prop.PropertyType.Name;
                        //if (prop.PropertyType.IsGenericType) { propTypeName = Nullable.GetUnderlyingType(prop.PropertyType).Name; }
                        //if (prop.PropertyType.IsGenericType && propTypeName != null && value == null) { prop.SetValue(obj, null, null); return; }
                        //if (prop.PropertyType.IsClass && value == null) { prop.SetValue(obj, null, null); return; }
                        prop.SetValue(obj, DBTable<T>.ConvertFieldValue(prop.PropertyType,value), null);
                    }
                    break;
                case MemberTypes.Field:
                    {
                        FieldInfo field = m as FieldInfo;
                        
                        field.SetValue(obj, DBTable<T>.ConvertFieldValue(field.FieldType,value));
                    }
                    break;
            }
            


        }

        protected static object ConvertFieldValue(Type fieldtype, object value)
        {
            object result = null;
            string propTypeName = fieldtype.Name;
            if (fieldtype.IsGenericType) { propTypeName = Nullable.GetUnderlyingType(fieldtype).Name; }
            if (fieldtype.IsGenericType && propTypeName != null && value == null) { return null; }
            if (fieldtype.IsClass && value == null) { return null; }

            switch (propTypeName.ToLower())
            {
                case "string": result = Convert.ToString(value).TrimEnd(); break;
                case "char": result = Convert.ToChar(value); break;
                case "byte": result = Convert.ToByte(value); break;
                case "short": result = Convert.ToByte(value); break;
                case "int": result = Convert.ToInt32(value); break;
                case "int16": result = Convert.ToInt16(value); break;
                case "int32": result = Convert.ToInt32(value); break;
                case "int64": result = Convert.ToInt64(value); break;
                case "uint16": result = Convert.ToUInt16(value); break;
                case "uint32": result = Convert.ToUInt32(value); break;
                case "uint64": result = Convert.ToUInt64(value); break;
                case "long": result = Convert.ToInt64(value); break;
                case "decimal": result = Convert.ToDecimal(value); break;
                case "single": result = Convert.ToSingle(value); break;
                case "double": result = Convert.ToDouble(value); break;
                case "boolean": result = Convert.ToBoolean(value); break;
                case "bool": result = Convert.ToBoolean(value); break;
                case "datetime": result = Convert.ToDateTime(value); break;
                case "guid": result = new Guid(Convert.ToString(value).TrimEnd()); break;
                case "date": result = Convert.ToDateTime(value); break;
                default:
                    if (fieldtype.IsEnum && value is string)
                        value = Enum.Parse(fieldtype, value.ToString());
                    result = value; break;
            }
            return result;
        }

        protected static object ConvertValue(object value, MemberInfo target)
        {
            object result = null;
            string targetType ="";
            switch ( target.MemberType)
            {
                case MemberTypes.Property:
                    {
                        PropertyInfo prop = target as PropertyInfo;
                        targetType = prop.PropertyType.Name;
                        break;
                    }

                case MemberTypes.Field:
                    {
                        FieldInfo prop = target as FieldInfo;
                        targetType = prop.FieldType.Name;
                        break;
                    }

            }
            switch (targetType.ToLower())
            {
                case "string": result = Convert.ToString(value).TrimEnd(); break;
                case "char": result = Convert.ToChar(value); break;
                case "byte": result = Convert.ToByte(value); break;
                case "short": result = Convert.ToByte(value); break;
                case "int": result = Convert.ToInt32(value); break;
                case "int16": result = Convert.ToInt16(value); break;
                case "int32": result = Convert.ToInt32(value); break;
                case "int64": result = Convert.ToInt64(value); break;
                case "uint16": result = Convert.ToUInt16(value); break;
                case "uint32": result = Convert.ToUInt32(value); break;
                case "uint64": result = Convert.ToUInt64(value); break;
                case "long": result = Convert.ToInt64(value); break;
                case "decimal": result = Convert.ToDecimal(value); break;
                case "single": result = Convert.ToSingle(value); break;
                case "double": result = Convert.ToDouble(value); break;
                case "boolean": result = Convert.ToBoolean(value); break;
                case "bool": result = Convert.ToBoolean(value); break;
                case "datetime": result = Convert.ToDateTime(value); break;
                case "guid": result = new Guid(Convert.ToString(value).TrimEnd()); break;
                case "date": result = Convert.ToDateTime(value); break;
                default: result = value; break;
            }
            return result;
        }

        public string ParseSql(string SqlText)
        {
            return this.ParseSql(SqlText, null);
        }
        /// <summary>
        /// Parse SQL and substitute SQLMacros. the Macros #__TableName and #__PK will be automaticly substituted if defined for the Table and do not need to be explicity included in the Macros array.
        /// </summary>
        /// <param name="SqlText">SQL statements</param>
        /// <param name="SQLMacros">Array of Macros to substitute</param>
        /// <returns></returns>
        public string ParseSql(string SqlText, SqlMacro[] SQLMacros)
        {
            if (SqlText.Trim().ToLower().StartsWith("from #__tablename")) SqlText = "SELECT * " + SqlText;
            List<SqlMacro> macros = new List<SqlMacro>();
            if (this._tableName != null)
                macros.Add(new SqlMacro<string>("#__") { MacroType = SqlMacro.MacroTypes.Identifier, Name = "TableName", Value = this._tableName });
                //SqlText = subTableNameRegex.Replace(SqlText, this._tableName);
            if (this._pk != null)
                macros.Add(new SqlMacro<string>("#__") { MacroType = SqlMacro.MacroTypes.Identifier, Name = "PK", Value = this._pk });
                //SqlText = subPKRegex.Replace(SqlText, this._pk);
            if (SQLMacros != null && SQLMacros.Length > 0) macros.AddRange(SQLMacros);
            if (macros != null && macros.Count > 0)
                foreach (SqlMacro m in macros)
                    SqlText = Regex.Replace(SqlText, m.Token + @"([^A-Za-z0-9]|$)", m.ToString() + @"$1");
                    //SqlText = SqlText.Replace(m.Token, m.ToString());
            return SqlText;
        }

        public delegate void BeforeSave_Delegate(T obj, bool isNew);
        public BeforeSave_Delegate BeforeSave;
        protected virtual void OnBeforeSave(T obj, bool isNew)
        {
            if (this.BeforeSave != null) this.BeforeSave(obj, isNew);
        }

        public virtual void Save(T obj){
            if (obj == null) return;

            bool isNew;
            bool isDirty = true; //defaults to true.
            if (obj is ISupportsIsDirty) isDirty = ((ISupportsIsDirty)obj).IsDirty();
            if (!isDirty) return;
      
            //if (_pk_memberinfo == null) return;

            isNew = this.IsNew(obj);

            this.OnBeforeSave(obj, isNew);

            string sqlPrefix = isNew ? "INSERT INTO #__TableName \n" : "UPDATE #__TableName SET \n";
            

            List<string> fields = new List<string>();
            List<string> values = new List<string>();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            Int32 pidx = 0;

            MemberInfo[] members = obj.GetType().GetMembers( BindingFlags.Instance | BindingFlags.Public);
            if (members == null || members.Length == 0) return;
            foreach (MemberInfo m in members) {
                switch (m.MemberType ){
                    case MemberTypes.Property:{
                        PropertyInfo pi = m as PropertyInfo;
                        string propName = pi.Name;
                        //object[] attrs = pi.GetCustomAttributes(typeof(NotPersistedAttribute),false);
                        //if (attrs != null && attrs.Length > 0) continue;
                        if (Attribute.IsDefined(m,typeof(NotPersistedAttribute))) continue;
                        object attr = Attribute.GetCustomAttribute(pi,typeof(FFLib.Attributes.MapsToAttribute), false);
                        if (attr != null) propName = ((FFLib.Attributes.MapsToAttribute)attr).PropertyName;
                        pidx++;
                        if (isNew){
                            fields.Add("[" + propName + "]");
                            values.Add("@p" + pidx.ToString()); 
                        } else {
                            fields.Add("[" + propName + "] = @p" + pidx.ToString());
                        }

                        if (pi.PropertyType.Equals(typeof(DateTime))
                            || (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                            && Nullable.GetUnderlyingType(pi.PropertyType).Equals(typeof(DateTime))))
                        {

                            if (pi.GetValue(obj,null)!= null && (DateTime)pi.GetValue(obj,null) == DateTime.MinValue) pi.SetValue(obj, null,null);
                        }
                        object propvalue = pi.GetValue(obj,null);
                        if (propvalue == null) propvalue = DBNull.Value;
                        else {
                            attr = Attribute.GetCustomAttribute(pi,typeof(FFLib.Data.Attributes.MaxLength), false);
                            if (attr != null) propvalue = propvalue.ToString().Left(((FFLib.Data.Attributes.MaxLength)attr).Value);
                            attr = Attribute.GetCustomAttribute(pi, typeof(FFLib.Data.Attributes.CastAs), false);
                            if (attr != null) propvalue = this.DBCastAs(((FFLib.Data.Attributes.CastAs)attr).DBType,propvalue);
                            }
                        sqlParams.Add(new SqlParameter("@p" + pidx.ToString(),propvalue ));
                    break;}
                    case MemberTypes.Field:{
                        FieldInfo fi = m as FieldInfo;
                        string fieldName = fi.Name;
                        if (Attribute.IsDefined(m, typeof(NotPersistedAttribute))) continue;
                        //object[] attrs = fi.GetCustomAttributes(typeof(NotPersistedAttribute),false);
                        //if (attrs != null && attrs.Length > 0) continue;
                        object attr = Attribute.GetCustomAttribute(fi,typeof(FFLib.Attributes.MapsToAttribute), false);
                        if (attr != null) fieldName = ((FFLib.Attributes.MapsToAttribute)attr).PropertyName;
                        pidx++;
                        if (isNew){
                            fields.Add("[" + fieldName + "]");
                            values.Add("@p" + pidx.ToString());
                        } else {
                            fields.Add("[" + fieldName + "] = @p" + pidx.ToString());
                        }
                        
                        if (fi.FieldType.Equals(typeof(DateTime)) 
                            || (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) 
                            && Nullable.GetUnderlyingType(fi.FieldType).Equals(typeof(DateTime)))){
                            if ((DateTime)fi.GetValue(obj) == DateTime.MinValue) fi.SetValue(obj,null);
                        }

                        object propvalue = fi.GetValue(obj);
                        if (propvalue == null) propvalue = DBNull.Value;
                        else
                        {
                            attr = Attribute.GetCustomAttribute(fi,typeof(FFLib.Data.Attributes.MaxLength), false);
                            if (attr != null) propvalue = propvalue.ToString().Left(((FFLib.Data.Attributes.MaxLength)attr).Value);
                        }
                        sqlParams.Add(new SqlParameter("@p" + pidx.ToString(), propvalue));
                        break;}
                    default: continue;
                }
            }

            if (fields.Count == 0) return;
            string sqlfields = isNew ? "(" + string.Join(",",fields.ToArray()) + ")" : string.Join(",",fields.ToArray());
            string sqlvalues = isNew ? " VALUES (" + string.Join(",", values.ToArray()) + ")" : "";
            string sqlCriteria = isNew ? "" : "\n WHERE #__PK = @pk";
            string sqlText = this.ParseSql(sqlPrefix + sqlfields + sqlvalues + sqlCriteria + (isNew & this.PK_IsDBIdentity ? "; SELECT SCOPE_IDENTITY();" : ""));

            sqlParams.Add(new SqlParameter("@pk", (int)GetPKValue(obj)));

            try
            {
                _dbProvider.CommandTimeout = this.CommandTimeout;
                if (isNew)
                {
                    var result = _dbProvider.DBInsert<decimal?>(_conn, sqlText, sqlParams.ToArray());
                    if (this.PK_IsDBIdentity) SetPKValue(obj, result);
                }
                else
                {
                    _dbProvider.DBUpdate(_conn, sqlText, sqlParams.ToArray());
                }
                //refresh from DB
                obj = this.Load((int)this.GetPKValue(obj));
            }
            finally
            {
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }

        }

        public virtual void Delete(T obj)
        {
            if (obj == null) return;

            if (this.IsNew(obj)) return;

            string sqlPrefix = "DELETE FROM #__TableName \n";

            string sqlCriteria = "\n WHERE #__PK = @pk";
            string sqlText = this.ParseSql(sqlPrefix + sqlCriteria);
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            sqlParams.Add(new SqlParameter("@pk", (int)GetPKValue(obj)));

            try
            {
                _dbProvider.CommandTimeout = this.CommandTimeout;
                _dbProvider.ExecuteNonQuery(_conn, sqlText, sqlParams.ToArray());
                
            }
            finally
            {
                if (_conn != null && !_conn.InTrx && _conn.State != ConnectionState.Closed) _conn.Close();
            }

        }

        public bool PK_IsDBIdentity
        {
            get
            {
                if (_pk_memberinfo == null) return false;
                return Attribute.IsDefined(_pk_memberinfo,typeof(FFLib.Data.Attributes.DBIdentity), false);
            }
        }

        public bool IsNew(T obj){
            bool isNew = false;
            if (obj is ISupportsIsNew) return ((ISupportsIsNew)obj).IsNew();
            if (_pk_memberinfo == null) return false;
            switch (_pk_memberinfo.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo pi = _pk_memberinfo as PropertyInfo;
                        if (pi.PropertyType.IsClass) isNew = pi.GetValue(obj,null) == null ? true : false;
                        else isNew = pi.GetValue(obj,null).Equals((Activator.CreateInstance(pi.PropertyType))) ? true : false;
                        break;
                    case MemberTypes.Field:
                        FieldInfo fi = _pk_memberinfo as FieldInfo;
                        if (fi.FieldType.IsClass) isNew = fi.GetValue(obj) == null ? true : false;
                        else isNew = fi.GetValue(obj).Equals(Activator.CreateInstance(fi.FieldType)) ? true : false;
                        break;
                }
            return isNew;
        }

        public object GetPKValue(T obj){
            if (this._pk_memberinfo == null) throw new InvalidOperationException("Primary Key has not been defined");
            switch (_pk_memberinfo.MemberType)
            {
                case MemberTypes.Property:
                    {
                        PropertyInfo prop = _pk_memberinfo as PropertyInfo;
                        return prop.GetValue(obj, null);
                    }

                case MemberTypes.Field:
                    {
                        FieldInfo prop = _pk_memberinfo as FieldInfo;
                        return prop.GetValue(obj);
                    }

            }
            return null;
        }

        public object SetPKValue(T obj, object val)
        {
            if (this._pk_memberinfo == null) throw new InvalidOperationException("Primary Key has not been defined");
            switch (_pk_memberinfo.MemberType)
            {
                case MemberTypes.Property:
                    {
                        PropertyInfo prop = _pk_memberinfo as PropertyInfo;
                        if (prop.PropertyType.IsValueType && (val == DBNull.Value || val == null) ) throw new ArgumentOutOfRangeException("Primary Key Cannot be set to Null or Empty");
                        prop.SetValue(obj,DBTable<T>.ConvertValue(val,_pk_memberinfo),null);
                        break;
                    }

                case MemberTypes.Field:
                    {
                        FieldInfo prop = _pk_memberinfo as FieldInfo;
                        if (prop.FieldType.IsValueType && val == DBNull.Value) throw new ArgumentOutOfRangeException("Primary Key Cannot be set to Null or Empty");
                        prop.SetValue(obj, DBTable<T>.ConvertValue(val, _pk_memberinfo));
                        break;
                    }

            }
            return null;
        }
        
        public object DBCastAs(DBType asType, object value)
        {
            switch (asType)
            {
                case DBType.Boolean: return (bool)value;
                case DBType.Char:
                    if (value.GetType().IsEnum) return (char)(int)value;
                    return (char)value;
                case DBType.CharEnum:
                    if (value == null) return null;
                    if (value.GetType().IsEnum) return (char)(int)value;
                    if (value is string) return ((int)((string)value)[0]).ToString();
                    return value;
                case DBType.Decimal: return (decimal)value;
                case DBType.Integer: return (int)value;
                case DBType.String: return (string)value;
                default: return value;
            }
        }
    }
}
