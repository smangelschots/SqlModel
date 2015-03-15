using System;
using Microsoft.SqlServer.Management.Smo;

namespace OfficeSoft.Data
{
    [Serializable]
    public class ForeignKeyColumnModel : BaseModel
    {
        public string ReferencedColumn { get; set; }

    }
}