/*******************************************************
 * Project: FFLib V1.0
 * Title: TableNameAttribute.cs
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

namespace FFLib.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MaxLength : System.Attribute
    {
        int _maxLength = 0;
        public MaxLength(int maxlength) 
        {
            if (maxlength < 0) maxlength = 0;
            _maxLength = maxlength; 
        }

        public int Value { get { return _maxLength; } }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PrimaryKeyAttribute : System.Attribute 
    {
        public PrimaryKeyAttribute(){}
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NotPersistedAttribute : System.Attribute
    {
        public NotPersistedAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DBIdentity : System.Attribute
    {
        public DBIdentity() { }
    }

    [AttributeUsage(AttributeTargets.Class )]
    public class DBTableNameAttribute : System.Attribute
    {
        string _tableName;
        public DBTableNameAttribute(string TableName)
        {
            _tableName = TableName;
        }

        public string TableName { get { return _tableName; } }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CastAs : System.Attribute
    {
        public DBType DBType;
        public CastAs(DBType asType) { DBType = asType; }
    }
}
