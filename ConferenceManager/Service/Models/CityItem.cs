using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceManager.Service.Database;
using ConferenceManager.Service.Support;

namespace TestNupi.Service.Models {
    public class CityItem : IdValueItem {

        public object region_id { get; set; }

        public CityItem(object _id, object _value, object regionId)
            : base(_id, _value) {
            region_id = regionId;
        }

        public static List<CityItem> getAll() {
            var table = TableManager.GetTable(NameDict.city);
            return table.Select().Select(createItem).ToList();
        }

        public static List<CityItem> getUsed(DateTime startDate) {
            var query = string.Format("{0} >= '{1}'", NameDict.event_date, startDate);
            var usedCityIdRows = TableManager.GetTable(NameDict.conference).Select(query);
            var idList = usedCityIdRows.Select(t => t[NameDict.city_id]).Distinct().ToList();
            if (idList.Count == 0) {
                return new List<CityItem>(0);
            } else {
                query = string.Format("{0} in ({1})", NameDict.id, string.Join(",", idList));
                var cityList = TableManager.GetTable(NameDict.city).Select(query);
                return cityList.Select(createItem).ToList();
            }
        }

        private static CityItem createItem(DataRow row) {
            return new CityItem(row[NameDict.id], row[NameDict.name], row[NameDict.region_id]);
        }
    }
}
