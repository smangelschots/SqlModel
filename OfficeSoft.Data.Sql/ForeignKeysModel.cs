using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace OfficeSoft.Data
{
    [Serializable]
    public class ForeignKeyModel : BaseModel, IForeignKeyModel
    {

        public ForeignKeyModel()
        {
            Columns = new List<ForeignKeyColumnModel>();
        }


        public List<ForeignKeyColumnModel> Columns { get; set; }
        public string DeleteAction { get; set; }
        public string UpdateAction { get; set; }
        public string GetReferencedKey()
        {
            return this.ReferencedKey;
        }

        public string GetReferencedTable()
        {
            return this.ReferencedTable;
        }

        public string ReferencedKey
        {
            get
            {
                var referencedKey = string.Empty;
                foreach (var foreignKeyColumnModel in Columns)
                {
                    referencedKey += foreignKeyColumnModel.ReferencedColumn;
                }


                return referencedKey;
            }
          
        }

        public string ReferencedTable { get; set; }

    }
}
