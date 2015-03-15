using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace OfficeSoft.Data.Sql
{
    public interface IDatabaseManager
    {

        List<string> GetDatabaseList();
        List<TableModel> GetTableList();
        DatabaseModel GetDatabaseModel();
        List<TableModel> GetViews();
        TableModel CreateTable(TableModel table);
        List<IColumnModel> GetColumns(string tablename);

       
    }
}