using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

using System.Data;

namespace OfficeSoft.Data.Sql
{
    [Serializable]
    public class SqlDatabaseManager : IDatabaseManager
    {
        private ServerConnection _conn;
        private Server _databaseServer;
        private Database _selectedDatabase;
        private void CreateDatabaseManager(string servername, string databasename, string username, string password)
        {
            string connstring = string.Empty;

            _conn = new ServerConnection();
            connstring = "Server=" + servername + ";Database=master";
            if (username == string.Empty || password == string.Empty)
                connstring += ";Trusted_Connection=yes;";
            else
                connstring += ";uid=" + username + ";pwd=" + password;

            _conn.ConnectionString = connstring;
            
            _databaseServer = new Server(_conn);
            
            Database database = DatabaseExists(databasename);
            if (database == null)
            {
                _selectedDatabase = CreateDatabase(databasename);
            }
            else
            {
                _selectedDatabase = database;
            }
        }
        private Database CreateDatabase(string databasename)
        {
            Database database = new Database(_databaseServer, databasename);
            _databaseServer.Databases.Add(database);
            database.Create();
            return database;
        }
        private Database DatabaseExists(string databasename)
        {
            foreach (Database item in _databaseServer.Databases)
            {
                if (item.Name.ToLower().Equals(databasename.ToLower()))
                    return item;
            }
            return null;
        }
        private Table TabelExist(string tabelname)
        {
            if (_selectedDatabase != null)
            {
                foreach (Table item in _selectedDatabase.Tables)
                {
                    if (item.Name.ToLower().Equals(tabelname.ToLower()))
                    {
                        return item;
                    }
                }

            }
            return null;
        }
        private TableModel ConvertTabelToModel(Table item)
        {
            var  table = new TableModel()
            {
                Name = item.Name,
                IdDb = item.ID,
                ColumnList = GetTableColumns(item.Name),
                ForeignKeyList = ConvertKeysToModels(item.ForeignKeys)
            };

            return table;
        }
        private TableModel ConvertViewToModel(View item)
        {
            var table = new TableModel()
            {
                Name = item.Name,
                IdDb = item.ID,
                ColumnList = GetViewColumns(item.Name),
            };

            return table;
        }

        private List<IColumnModel> GetViewColumns(string viewname)
        {
            var columns = new List<IColumnModel>();
            var view = ViewExist(viewname);
            if (view == null)
            {
                return null;
            }

            foreach (Column item in view.Columns)
            {
                var column = ConvertColumnToColumnModel(item);
                columns.Add(column);
            }
            return columns;
        }

        private View ViewExist(string viewname)
        {
            if (_selectedDatabase != null)
            {
                foreach (View item in _selectedDatabase.Views)
                {
                    if (item.Name.Equals(viewname))
                    {
                        return item;
                    }
                }

            }
            return null;
        }


        private List<ForeignKeyModel> ConvertKeysToModels(ForeignKeyCollection foreignKeys)
        {
            var models = new List<ForeignKeyModel>();
            foreach (ForeignKey foreignKey in foreignKeys)
            {
                var model = new ForeignKeyModel();
                var columnModelsmModels = new List<ForeignKeyColumnModel>();
                foreach (ForeignKeyColumn item in foreignKey.Columns)
                {
                    if (item.Parent.ReferencedTable == foreignKey.ReferencedTable)
                    {
                        var key = new ForeignKeyColumnModel();
                        key.IdDb = item.ID;
                        key.Name = item.Name;
                        key.ReferencedColumn = item.ReferencedColumn;
                        columnModelsmModels.Add(key);
                    }
                }

                model.Columns = columnModelsmModels;
                model.Name = foreignKey.Name;
                model.CreateDate = foreignKey.CreateDate;
                model.DeleteAction = foreignKey.DeleteAction.ToString();
                model.IdDb = foreignKey.ID;
                model.ReferencedTable = foreignKey.ReferencedTable;
                model.UpdateAction = foreignKey.UpdateAction.ToString();
                models.Add(model);
            }

            return models;

        }
        private ColumnModel ConvertColumnToColumnModel(Column item)
        {
            var column = new ColumnModel();
            column.Name = item.Name;
            column.IdDb = item.ID;
          
            column.DataType = new DataTypeModel()
            {
                Name = item.DataType.Name,
                MaximumLength = item.DataType.MaximumLength,
                NumericPrecision = item.DataType.NumericPrecision,
                NumericScale = item.DataType.NumericScale,
                CSharpType = item.DataType.SqlDataType.ToCSharpType(),
                SqlDataType = item.DataType.SqlDataType.ToString(),
            };
            column.DefaultValue = item.Default;
            column.RowGuidCol = item.RowGuidCol;
            column.Identity = item.Identity;
            column.Nullable = item.Nullable;
            column.Computed = item.Computed;
            column.IsPrimaryKey = item.InPrimaryKey;
            column.IsForeignKey = item.IsForeignKey;
            column.IsFullTextIndexed = item.IsFullTextIndexed;
            column.IsPersisted = item.IsPersisted;
            column.IsFileStream = item.IsFileStream;
            return column;
        }

        public SqlDatabaseManager(string servername, string databasename)
        {
            CreateDatabaseManager(servername, databasename, string.Empty, string.Empty);
        }
        public SqlDatabaseManager(string servername, string databasename, string username, string password)
        {
            CreateDatabaseManager(servername, databasename, username, password);
        }



        public static List<string> GetServerList()
        {
            List<string> ReturnList = new List<string>();
            try
            {
                DataTable dtSQLServers = SmoApplication.EnumAvailableSqlServers(false);
                foreach (DataRow drServer in dtSQLServers.Rows)
                {
                    String ServerName;
                    ServerName = drServer["Server"].ToString();

                    if (drServer["Instance"] != null && drServer["Instance"].ToString().Length > 0)
                        ServerName += @"\" + drServer["Instance"].ToString();

                    if (ReturnList.IndexOf(ServerName) < 0)
                    {
                        ReturnList.Add(ServerName);
                    }
                }
            }
            catch (SmoException smoException)
            {

            }
            catch (Exception exception)
            {

            }
            return ReturnList;
        }
        public List<string> GetDatabaseList()
        {
            if (_databaseServer != null)
            {
                List<string> databaselist = new List<string>();
                foreach (Database item in _databaseServer.Databases)
                {
                    if (item.IsSystemObject == false)
                        databaselist.Add(item.Name);
                }
                return databaselist;
            }
            return null;
        }
        public List<TableModel> GetTableList()
        {
            if (_selectedDatabase != null)
            {
                List<TableModel> tabels = new List<TableModel>();

                foreach (Table item in _selectedDatabase.Tables)
                {
                    if (item.IsSystemObject == false)
                    {

                        tabels.Add(ConvertTabelToModel(item));

                    }

                }
                return tabels;
            }
            return null;
        }
        public List<TableModel> GetViews()
        {
            if (_selectedDatabase != null)
            {
                List<TableModel> views = new List<TableModel>();
                foreach (View item in _selectedDatabase.Views)
                {
                    if (item.IsSystemObject == false)
                    {

                        views.Add(ConvertViewToModel(item));
                    }

                }
                return views;
            }
            return null;
        }



        public DatabaseModel GetDatabaseModel()
        {
            return new DatabaseModel()
            {
                Name = _selectedDatabase.Name,
                TabelModelList = GetTableList()
            };
        }
        public TableModel CreateTable(TableModel tableModel)
        {
            Table table = null;
            if (_selectedDatabase != null)
            {

                if (tableModel.ColumnList.Count > 0)
                {
                    table = TabelExist(tableModel.Name);
                    if (table == null)
                    {
                        table = new Table(_selectedDatabase, tableModel.Name);
                        foreach (var columnModel in tableModel.ColumnList)
                        {
                            var column = CreateNewColumn(table, columnModel, tableModel);
                        }
                        var prop = new ExtendedProperty(table,"id");
                        prop.Value = tableModel.Id;
                        table.ExtendedProperties.Add(prop);
                        table.Create();
                    }
                    else
                    {
                        foreach (var columnModel in tableModel.ColumnList)
                        {

                            Column column = null;
                            column = ColumnExists(table, columnModel.Name);
                            if (column == null)
                            {
                                column = CreateNewColumn(table, columnModel, tableModel);
                            }
                            else
                            {
                                
                                column.Nullable = columnModel.Nullable;
                                column.DataType = GetSqlType(columnModel.GetDataType());
                                
                                column.Alter();
                            }
                        }
                        table.Alter();
                    }


                    var primaryKeys = tableModel.GetPrimaryKeys();
                    if (tableModel.GetPrimaryKeys().Count > 0)
                    {
                        var indexid = "PK_" + table.Name;
                        Index index = IndexExists(table, indexid);
                        if (index == null)
                        {
                            index = new Index(table, indexid);
                            table.Indexes.Add(index);
                            foreach (var keys in primaryKeys)
                            {
                                index.IndexedColumns.Add(new IndexedColumn(index, keys.Name));
                            }
                            index.IsClustered = true;
                            index.IsUnique = true;
                            index.IndexKeyType = IndexKeyType.DriPrimaryKey;
                            table.Alter();
                        }
                        else
                        {

                            //TODO wijzeging
                        }
                    }
                    return tableModel;
                }
            }
            return null;
        }

        public List<IColumnModel> GetColumns(string tablename)
        {
            throw new NotImplementedException();
        }

        private Index IndexExists(Table table, string name)
        {
            foreach (Index index in table.Indexes)
            {
                if (index.Name == name)
                    return index;
            }

            return null;
        }

        private Column CreateNewColumn(Table table, IColumnModel columnModel, TableModel tableModel)
        {
            Column column = new Column(table, columnModel.Name, GetSqlType(columnModel.GetDataType()));


            var prop = new ExtendedProperty(column,"id");
            prop.Value = columnModel.Id;
            column.ExtendedProperties.Add(prop);
            column.Nullable = columnModel.Nullable;

            table.Columns.Add(column);
            return column;

        }

        private Column ColumnExists(Table table, string name)
        {
            foreach (Column column in table.Columns)
            {
                if (column.Name == name)
                    return column;
            }

            return null;
        }

        public DataType GetSqlType(IDataTypeModel dataTypeModel)
        {
            if (dataTypeModel == null) return null;

            if (!string.IsNullOrEmpty(dataTypeModel.SqlDataType))
            {

                try
                {
                    var en = (SqlDataType)Enum.Parse(typeof(SqlDataType), dataTypeModel.SqlDataType, true);

                    if (dataTypeModel.SqlDataType == SqlDataType.NVarCharMax.ToString())
                        return new DataType(en);


                    if (dataTypeModel.MaximumLength > 0)
                        return new DataType(en, dataTypeModel.MaximumLength);


                    if (dataTypeModel.NumericPrecision > 0 && dataTypeModel.NumericScale > 0)
                        return new DataType(en, dataTypeModel.NumericPrecision, dataTypeModel.NumericScale);

                    if (dataTypeModel.NumericPrecision > 0)
                        return new DataType(en, dataTypeModel.NumericPrecision);

                    if (dataTypeModel.NumericScale > 0)
                        return new DataType(en, dataTypeModel.NumericScale);

                    return new DataType(en);
                }
                catch (Exception)
                {


                }
            }

            switch (dataTypeModel.CSharpType.Name)
            {
                case "String":
                    return DataType.NVarChar(dataTypeModel.MaximumLength);

                case "Guid":
                    return DataType.UniqueIdentifier;

                case "Byte[]":
                    return DataType.VarBinaryMax;

            }

            return DataType.NVarChar(50);
        }


        //public void CreateColumns(string tabelname)
        //{
        //    Column column = null;
        //    //Table tabel = CreateTabel(tabelname);

        //    //if (SelectedDatabase != null)
        //    //{
        //    //    column = new Column(tabel, "test2", DataType.VarChar(50));
        //    //    tabel.Columns.Add(column);
        //    //    tabel.Alter();
        //    //}

        //}
        public List<IColumnModel> GetTableColumns(string tabelname)
        {
            var columns = new List<IColumnModel>();
            var table = TabelExist(tabelname);
            if (table == null)
            {
                return null;
            }

            foreach (Column item in table.Columns)
            {
                var column = ConvertColumnToColumnModel(item);
                columns.Add(column);
            }
            return columns;
        }


        public void CreateRelation(TableModel model)
        {
           


            foreach (var foreignKeyModel in model.ForeignKeyList)
            { 
                var table = TabelExist(foreignKeyModel.ReferencedTable);
                var id = string.Format("FK_{0}_{1}_{2}", model.Name, foreignKeyModel.ReferencedTable, foreignKeyModel.ReferencedKey);
                if (!table.ForeignKeys.Contains(id))
                {
                    ForeignKey foreignKey = new ForeignKey(table, id);
                    foreach (var foreignKeyColumnModel in foreignKeyModel.Columns)
                    {
                        ForeignKeyColumn foreignKeyColumn = new ForeignKeyColumn(foreignKey, foreignKeyColumnModel.ReferencedColumn, foreignKeyColumnModel.Name);
                        foreignKey.Columns.Add(foreignKeyColumn);
                    }
                    var prop = new ExtendedProperty(foreignKey,"id");
                    prop.Value = foreignKeyModel.Id;
                    foreignKey.ExtendedProperties.Add(prop);

                    foreignKey.ReferencedTable = model.Name;

                    foreignKey.Create();
                }
                else
                {
                    //YODO aanpassen
                }
               
            

            }
        }

        public void CloseConnection()
        {
           
            _conn.Disconnect();
           
        }
        
    }
}
