using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonConverterSqlite.Model
{
    public class TableInfo
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "rowCount")]
        public int RowCount { get; set; }

        [JsonProperty(PropertyName = "columnTypes")]
        public Dictionary<String, String> ColumnTypes { get; set; }

        [JsonProperty(PropertyName = "schema")]
        public String Schema { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Dictionary<String, Object>[] Data { get; set; }
    }
}
