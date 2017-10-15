using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceManager.Service.Database;
using ConferenceManager.Service.Support;

namespace TestNupi.Service.Models {
    public class IdValueItem : IdItem {

        public object value { get; set; }

        public IdValueItem(object _id, object _value)
            : base(_id) {
            value = _value;
        }

        public static List<IdValueItem> getAll(string tableName, string valueColumnName) {
            var table = TableManager.GetTable(tableName);
            return table.Select().Select(t => (IdValueItem)createItem(t, valueColumnName)).ToList();
        }

        protected static IdValueItem createItem(DataRow row, string valueColumnName) {
            return new IdValueItem(row[NameDict.id], row[valueColumnName]);
        }
    }
}
