using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceManager.Service.Database;
using ConferenceManager.Service.Support;

namespace TestNupi.Service.Models {
    public class ConferenceItem : IdItem {

        public object name { get; set; }

        public object date { get; set; }

        public object city { get; set; }

        public object region { get; set; }

        public object city_id { get; set; }

        public object location { get; set; }

        public object section { get; set; }

        public ConferenceItem(object _id, object _name, object eventDate, object address, object cityId, object sectionName)
            : base(_id) {
            name = _name;
            date = eventDate;
            location = address;
            city_id = cityId;
            var cityRow = TableManager.SelectRowByPrimaryKey(NameDict.city, city_id);
            city = cityRow[NameDict.name];
            region = KeyValueSwitch.GetValueByKey(NameDict.city, NameDict.region_id, cityRow[NameDict.region_id]);
            section = sectionName;
        }

        /// <summary>
        /// Получить все конференции по секции начиная с определенной даты
        /// </summary>
        public static List<ConferenceItem> getBySection(object sectionId, string sectionName, DateTime startDate, int? cityId = null) {
            var linkRows = TableManager.SelectRowsFromTable(NameDict.section_conference_link, NameDict.section_id, sectionId);
            var result = new List<ConferenceItem>(linkRows.Length);
            var idList = linkRows.Select(t => t[NameDict.conference_id]).ToList();
            if (idList.Count > 0) {
                var query = string.Format("{0} >= '{2}' and {1} in ({3})", NameDict.event_date, NameDict.id, startDate, string.Join(",", idList));
                if (cityId.HasValue && cityId.Value > 0)
                    query += " and " + NameDict.city_id + " = " + cityId.Value;
                var conferenceRows = TableManager.GetTable(NameDict.conference).Select(query,NameDict.order_number);
                foreach (DataRow row in conferenceRows) {
                    var newItem = createItem(row, sectionName);
                    result.Add(newItem);
                }
            }
            return result;
        }

        public static ConferenceItem saveOrUpdate(ConferenceItem changedItem, DataRow row, string sectionName) {
            bool isNew = row == null;
            var newRow = TableManager.GetTable(NameDict.conference).NewRow();
            newRow[NameDict.id] = isNew ? TableManager.GetNewId(NameDict.conference) : row[NameDict.id];
            newRow[NameDict.name] = changedItem.name;
            newRow[NameDict.event_date] = changedItem.date;
            newRow[NameDict.city_id] = changedItem.city;
            newRow[NameDict.address] = changedItem.location;
            if (isNew)
                TableManager.InsertRow(row);
            else
                TableManager.UpdateRecord(row, newRow);
            return createItem(newRow, sectionName);
        }

        private static ConferenceItem createItem(DataRow row, string sectionName) {
            var newItem = new ConferenceItem(row[NameDict.id], row[NameDict.name], row[NameDict.event_date], row[NameDict.address], row[NameDict.city_id], sectionName);
            return newItem;
        }
    }
}
