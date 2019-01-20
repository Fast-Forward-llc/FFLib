using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLibUnitTests.Mocks
{
    public class IDataReader: System.Data.IDataReader 
    {
#region "Classes"
        public class ResultSet{
            public string[] FieldNames;
            public object[][] Values;

            public ResultSet(string[] FieldNames, object[][] Values)
            {
                this.FieldNames = FieldNames;
                this.Values = Values;
            }
        }
#endregion
        private bool _open = false;
        private int CurrResultIdx = -1;
        private int CurrRowIdx = -1;
        private int CurrValueIdx = 0;

        public List<ResultSet> ResultSets = new List<ResultSet>();

        public void Close()
        {
            //throw new NotImplementedException();
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public System.Data.DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { return !_open; }
        }

        public bool NextResult()
        {
            CurrResultIdx++;
            if (ResultSets.Count < CurrResultIdx) return true;
            return false;
        }

        public bool Read()
        {
            CurrValueIdx++;
            if (ResultSets.Count < CurrResultIdx && ResultSets[CurrResultIdx].Values.Length < CurrRowIdx) return true;
            return false;
        }

        public int RecordsAffected { get; set; }

        public void Dispose()
        {
            
        }

        public int FieldCount
        {
            get { return ResultSets[CurrResultIdx].FieldNames.Length; }
        }

        public bool GetBoolean(int i)
        {
            return (bool)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public byte GetByte(int i)
        {
            return (byte)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public double GetDouble(int i)
        {
            return (double)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            return (float)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public short GetInt16(int i)
        {
            return (short)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public int GetInt32(int i)
        {
            return (int)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public long GetInt64(int i)
        {
            return (long)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public string GetName(int i)
        {
            return (string)ResultSets[CurrResultIdx].FieldNames[i];
        }

        public int GetOrdinal(string name)
        {
            int i = 0;
            for (int x = 0; x < ResultSets[CurrResultIdx].FieldNames.Length ; x++)
            {
                if (name == ResultSets[CurrResultIdx].FieldNames[x]) i = x;
            }
            return (int)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public string GetString(int i)
        {
            return (string)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public object GetValue(int i)
        {
            return (object)ResultSets[CurrResultIdx].Values[CurrRowIdx][i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }
    }
}
