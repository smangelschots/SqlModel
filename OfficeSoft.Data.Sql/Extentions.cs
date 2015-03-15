using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace OfficeSoft.Data
{
    public static class Extentions
    {
        public static Type ToCSharpType(this SqlDataType sqlDataType)
        {
            switch (sqlDataType)
            {
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                case SqlDataType.Text:
                case SqlDataType.None:
                case SqlDataType.Xml:
                    return typeof(string);
                case SqlDataType.BigInt:
                    return typeof(long);
                case SqlDataType.Binary:
                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.Variant:
                case SqlDataType.UserDefinedTableType:
                case SqlDataType.Geometry:
                case SqlDataType.Geography:
                case SqlDataType.HierarchyId:
                case SqlDataType.SysName:
                    return typeof(object);
                case SqlDataType.Bit:
                    return typeof(bool);
                case SqlDataType.Char:
                    return typeof(char);
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime2:

                    return typeof(DateTime);
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                    return typeof(decimal);
                case SqlDataType.Float:
                    return typeof(double);
                case SqlDataType.Image:
                case SqlDataType.Timestamp:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                    return typeof(byte[]);
                case SqlDataType.Int:
                    return typeof(int);
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return typeof(decimal);
                case SqlDataType.Real:
                    return typeof(float);
                case SqlDataType.SmallInt:
                    return typeof(short);
                case SqlDataType.TinyInt:
                    return typeof(byte);
                case SqlDataType.UniqueIdentifier:
                    return typeof(Guid);
                case SqlDataType.DateTimeOffset:
                    return typeof(DateTimeOffset);
                default:
                    throw new ArgumentOutOfRangeException("sqlDataType");
            }
        }
    }
}
