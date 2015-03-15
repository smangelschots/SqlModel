using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeSoft.Data
{
    [Serializable]
    public class ColumnModel : BaseModel, IColumnModel
    {
        public DataTypeModel DataType { get; set; }
        public string DefaultValue { get; set; }
        public bool RowGuidCol { get; set; }
        public bool Identity { get; set; }
        public bool Nullable { get; set; }
        public bool Computed { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsFullTextIndexed { get; set; }
        public bool IsPersisted { get; set; }
        public bool IsFileStream { get; set; }
        public IDataTypeModel GetDataType()
        {
            return DataType;
        }
    }
}
