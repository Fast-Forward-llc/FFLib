using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FFLib.Extensions;

using System.Text;
using System.Data;

namespace FFLib.Data
{
    /// <summary>
    /// Helper class to generate a dataset and IDataReader suitable to use as the return value of a mocked IDBProvider.
    /// </summary>
    /// <typeparam name="T">Type of the DBTable that is used with the mocked IDBProvider</typeparam>
    public static class MockDBData<T>
    {
        /// <summary>
        /// Import data for use as a datasource of an IDbProvider. The returned IDataReader can be supplied as the return value of a mocked IDbProvider's ExecuteReader method.
        /// </summary>
        /// <param name="data">array of dynamic objects to be imported as the data source</param>
        /// <returns>DataReader loaded from the imported dynamic data</returns>
        public static IDataReader ImportTableData(dynamic[] data)
        {
            var d1 = data[0];
            System.Data.DataTable dr = new System.Data.DataTable();
            var fieldList = new Dictionary<string, MemberInfo>();
            MemberInfo[] miList = d1.GetType().GetMember("*", MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public);
            foreach (var mi in miList)
            {
                fieldList.AddOrReplace(mi.Name, mi);
            }
            foreach (var fieldName in fieldList.Keys)
                dr.Columns.Add(fieldName, (fieldList[fieldName].MemberType == MemberTypes.Property ? ((PropertyInfo)fieldList[fieldName]).PropertyType : ((FieldInfo)fieldList[fieldName]).FieldType));
            foreach (var d in data)
            {
                System.Data.DataRow drRow = dr.NewRow();
                foreach (var fieldName in fieldList.Keys)
                {
                    var mi2 = fieldList[fieldName];
                    if (mi2 == null) continue;
                    switch (mi2.MemberType)
                    {
                        case MemberTypes.Field:
                            {
                                drRow[fieldName] = ((FieldInfo)mi2).GetValue(d); break;
                            }
                        case MemberTypes.Property:
                            {
                                drRow[fieldName] = ((PropertyInfo)mi2).GetValue(d, null); break;
                            }
                    }
                }
                dr.Rows.Add(drRow);
            }

            return dr.CreateDataReader();

        }

        /// <summary>
        /// Import data for use as a datasource of an IDbProvider. The returned IDataReader can be supplied as the return value of a mocked IDbProvider's ExecuteReader method.
        /// </summary>
        /// <param name="data">array of dynamic objects to be imported as the data source</param>
        /// <returns>DataReader loaded from the imported dynamic data</returns>
        public static IDataReader ImportTableData(dynamic data)
        {
            var X = new dynamic[] { data };
            return MockDBData<T>.ImportTableData((dynamic[])X);
        }

        /// <summary>
        /// Import data for use as a datasource of an IDbProvider. The returned IDataReader can be supplied as the return value of a mocked IDbProvider's ExecuteReader method.
        /// </summary>
        /// <param name="data">array of typed objects to be imported as the data source</param>
        /// <returns>DataReader loaded from the imported typed data</returns>
        public static IDataReader ImportTableData(T[] data)
        {
            var X = new List<dynamic>();
            foreach (var d in data) X.Add((dynamic)d);
            return MockDBData<T>.ImportTableData(X.ToArray());
        }

        /// <summary>
        /// Import data for use as a datasource of an IDbProvider. The returned IDataReader can be supplied as the return value of a mocked IDbProvider's ExecuteReader method.
        /// </summary>
        /// <param name="data">array of typed objects to be imported as the data source</param>
        /// <returns>DataReader loaded from the imported typed data</returns>
        public static IDataReader ImportTableData(T data)
        {
            var X = new T[] { data };
            return MockDBData<T>.ImportTableData((dynamic)X);
        }



    }
}
