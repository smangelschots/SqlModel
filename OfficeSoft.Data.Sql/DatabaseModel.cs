using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeSoft.Data
{
    [Serializable]
    public class DatabaseModel : BaseModel
    {
        public List<TableModel> TabelModelList { get; set; }
    }
}
