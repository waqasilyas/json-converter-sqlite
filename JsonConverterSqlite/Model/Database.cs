using System;
using System.Collections.Generic;

using System.Data.SQLite;
using Newtonsoft.Json;

namespace JsonConverterSqlite.Model
{
    public class Database : IDisposable
    {
        public static readonly String[] DATABASE_METADATA = {
            "schema_version",
            "user_version",
            "application_id"
        };

        public Database(String sqliteFile)
        {
            Connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Read Only=True;", sqliteFile));
            Connection.Open();

            _info = GetDatabaseInfo();
            _tables = getTableInfo();
        }

        #region Properties

        private SQLiteConnection Connection;
        private Dictionary<String, String> _info;
        private TableInfo[] _tables;

        [JsonProperty(PropertyName = "databaseInfo")]
        public Dictionary<String, String> DatabaseInfo
        {
            get
            {
                return _info;
            }
        }

        [JsonProperty(PropertyName = "tables")]
        public TableInfo[] Tables
        {
            get
            {
                return _tables;
            }
        }

        #endregion

        #region Public

        public Dictionary<String, String> GetDatabaseInfo()
        {
            Dictionary<String, String> info = new Dictionary<string, string>();

            foreach (String pragma in DATABASE_METADATA)
            {
                String[] result = ExecutePragma(pragma);
                if (result != null && result[0].Length > 0)
                    info.Add(pragma, result[0]);
            }

            return info;
        }

        public TableInfo[] getTableInfo()
        {
            List<TableInfo> tables = new List<TableInfo>();

            String sql = "SELECT name, sql FROM sqlite_master WHERE type='table';";
            Object[][] result = ExecuteSql(sql);
            if (result == null)
                tables.ToArray();

            for (int i = 0; i < result.Length; i++)
            {
                TableInfo table = new TableInfo();
                table.Name = result[i][0].ToString();
                table.Schema = result[i][1].ToString();
                table.RowCount = GetRowCount(table.Name);
                table.ColumnTypes = GetColumnTypes(table.Name);
                table.Data = GetTableContent(table.Name, new List<string>(table.ColumnTypes.Keys));

                tables.Add(table);
            }

            return tables.ToArray();
        }

        public Dictionary<String, String> GetColumnTypes(String tableName)
        {
            Dictionary<String, String> columnTypes = new Dictionary<String, String>();

            String tableInfo = String.Format("PRAGMA table_info({0})", tableName);
            Object[][] info = ExecuteSql(tableInfo);
            if (info != null)
            {
                for (int j = 0; j < info.Length; j++)
                    columnTypes.Add(info[j][1].ToString(), info[j][2].ToString());
            }

            return columnTypes;
        }

        public Dictionary<String, Object>[] GetTableContent(String tableName, List<String> columns)
        {
            List<Dictionary<String, Object>> content = new List<Dictionary<String, Object>>();

            if (columns == null || columns.Count == 0)
                columns = new List<String>(GetColumnTypes(tableName).Keys);

            String sql = String.Format("SELECT * FROM {0}", tableName);
            Object[][] data = ExecuteSql(sql);
            for (int i = 0; i < data.Length; i++)
            {
                Dictionary<String, Object> row = new Dictionary<String, Object>();
                content.Add(row);

                for (int j = 0; j < data[i].Length; j++)
                {
                    row.Add(columns[j], data[i][j]);
                }
            }

            return content.ToArray();
        }

        public int GetRowCount(String table)
        {
            String sql = String.Format("SELECT count(*) FROM {0};", table);
            String[] result = ExecuteSql(sql, 0);
            if (result != null && result[0].Length != 0)
                return Int32.Parse(result[0]);

            return 0;
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        #endregion

        #region Private

        private String[] ExecutePragma(String pragma)
        {
            String sql = "PRAGMA {0}";
            String[] result = ExecuteSql(String.Format(sql, pragma), 0);
            if (result != null && result.Length > 0)
                return result;

            return null;
        }

        private String[] ExecuteSql(String sql, int column)
        {
            Object[][] data = ExecuteSql(sql);
            String[] list = new String[data.Length];

            for (int i = 0; i < list.Length; i++)
                list[i] = data[i][column].ToString();

            return list;
        }

        private Object[][] ExecuteSql(String sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Object[]> rows = new List<Object[]>();

            while (reader.Read())
            {
                Object[] cols = new Object[reader.FieldCount];

                for (int i = 0; i < cols.Length; i++)
                    cols[i] = reader.GetValue(i);

                rows.Add(cols);
            }

            return rows.ToArray();
        }

        #endregion
    }
}
