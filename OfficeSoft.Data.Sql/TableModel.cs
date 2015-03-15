using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeSoft.Data
{
    [Serializable]
    public class TableModel : BaseModel, ITableModel
    {

        public TableModel()
        {
            ColumnList = new List<IColumnModel>();
            ForeignKeyList = new List<ForeignKeyModel>();
        }


        public List<IColumnModel> ColumnList { get; set; }
        public List<ForeignKeyModel> ForeignKeyList { get; set; }

        public List<IColumnModel> GetPrimaryKeys()
        {
            return ColumnList.Where(c => c.IsPrimaryKey).ToList<IColumnModel>();
        }

        public List<IForeignKeyModel> GetForeignKeyModels()
        {
            return ForeignKeyList.Cast<IForeignKeyModel>().ToList();
        }
    }
}
