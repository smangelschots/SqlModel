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

        public static string GetConvertType(this IColumnModel column)
        {

            var typeName = column.GetDataType().CSharpType.FullName;

            switch (typeName)
            {
                case "System.Xml.XmlDocument":
                case "System.String":
                    typeName = "String";
                    break;
                case "System.Byte[]":
                    typeName = "Binary";
                    break;
                case "System.Int32":
                case "System.Int64":
                    typeName = "Int";
                    break;

                case "System.Single":
                    typeName = "Float";
                    break;
                    
                case "System.Object":
                    typeName = "Object";
                    break;
                case "System.DateTime":
                    typeName = "DateTime";
                    break;
                case "System.Boolean":
                    typeName = "Bool";
                    break;
                case "System.Guid":
                    typeName = "Guid";
                    break;
                case "System.Decimal":
                    typeName = "Currency";
                    break;
                case "System.Double":
                    typeName = "Double";
                    break;
                case "System.Foat":
                    typeName = "Float";
                    break;
                case "System.Char":
                    typeName = "Char";
                    break;
                default:

                    break;
            }



            if (column.Nullable)
            {
                if (typeName != "String")
                    if (typeName != "Binary")
                        if(typeName != "Currency")
                            if(typeName != "Bool")
                        typeName = string.Format("{0}Nullable", typeName);
            }

            return typeName;
        }


        private static string GetTypeName(string typeName)
        {
            
            switch (typeName)
            {
                case "System.String":
                    typeName = "string";
                    break;
                case "System.Byte[]":
                    typeName = "System.Byte[]";
                    break;
                case "System.Int32":
                case "System.Int64":
                    typeName = "int";
                    break;
                case "System.Xml.XmlDocument":
                    typeName = "string";
                    break;
                case "System.Object":
                    typeName = "Object";
                    break;
                case "System.DateTime":
                    typeName = "DateTime";
                    break;
                case "System.Boolean":
                    typeName = "bool";
                    break;
                case "System.Guid":
                    typeName = "Guid";
                    break;
                case "System.Decimal":
                    typeName = "decimal";
                    break;
                case "System.Double":
                    typeName = "double";
                    break;
                case "System.Single":
                case "System.Foat":
                    typeName = "float";
                    break;
                case "System.Char":
                    typeName = "char";
                    break;

                default:

                    break;
            }
            return typeName;
        }


        public static string GetSystemType(this IColumnModel column)
        {

            var typeName = GetTypeName(column.GetDataType().CSharpType.FullName);

            if (column.Nullable)
            {
                if (typeName != "string")
                    if(typeName != "System.Byte[]")
                    typeName = string.Format("Nullable<{0}>", typeName);
            }

            return typeName;
        }



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
                    return typeof(float);
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
