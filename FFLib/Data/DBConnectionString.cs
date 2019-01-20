using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Data
{
    public interface IDBConnectionString
    {
        string GetValue();
    }

    public class DBConnectionString : IDBConnectionString
    {
        string _connString;

        public DBConnectionString(string conn)
        {
            _connString = conn;
        }

        public string GetValue() { return _connString; }
    }
}
