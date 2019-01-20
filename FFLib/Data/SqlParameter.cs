using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Data
{
    public class SqlParameter
    {

        public SqlParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; set; }

        public static SqlParameter[] FromDynamic(dynamic sqlParams)
        {
            var spList = new List<SqlParameter>();
            if (sqlParams == null) return spList.ToArray();
            foreach (var prop in sqlParams.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                spList.Add(new SqlParameter("@"+prop.Name, prop.GetValue(sqlParams, null)));
            return spList.ToArray();
        }
    }

    public class SqlParameter<T> : SqlParameter
    {
        public SqlParameter(string name, T value) : base(name, value) { }
    }
}
