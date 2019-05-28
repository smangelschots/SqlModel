﻿using System;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Smo;

namespace OfficeSoft.Data
{
    [Serializable]
    public class DataTypeModel : IDataTypeModel
    {
        public DataTypeModel()
        {
            Id = Guid.NewGuid();
        }


        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MaximumLength { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
        public Type CSharpType { get; set; }
        public string SqlDataType { get; set; }



    }


    public enum SqlDataTypeLocal
    {
        None = 0,
        BigInt = 1,
        Binary = 2,
        Bit = 3,
        Char = 4,
        DateTime = 6,
        Decimal = 7,
        Float = 8,
        Image = 9,
        Int = 10,
        Money = 11,
        NChar = 12,
        NText = 13,
        NVarChar = 14,
        NVarCharMax = 15,
        Real = 16,
        SmallDateTime = 17,
        SmallInt = 18,
        SmallMoney = 19,
        Text = 20,
        Timestamp = 21,
        TinyInt = 22,
        UniqueIdentifier = 23,
        UserDefinedDataType = 24,
        UserDefinedType = 25,
        VarBinary = 28,
        VarBinaryMax = 29,
        VarChar = 30,
        VarCharMax = 31,
        Variant = 32,
        Xml = 33,
        SysName = 34,
        Numeric = 35,
        Date = 36,
        Time = 37,
        DateTimeOffset = 38,
        DateTime2 = 39,
        UserDefinedTableType = 40,
        HierarchyId = 41,
        Geometry = 42,
        Geography = 43,
    }
}