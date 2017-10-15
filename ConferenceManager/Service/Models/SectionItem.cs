using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceManager.Service.Database;
using ConferenceManager.Service.Support;

namespace TestNupi.Service.Models {
    public class SectionItem : IdItem {

        public object name { get; set; }

        public List<ConferenceItem> info { get; set; }

        public SectionItem(object _id, object _name)
            : base(_id) {
            name = _name;
        }

        public static List<SectionItem> getFull(bool shortInfo, DateTime? startDate = null, int? cityId = null) {
            var sectionTable = TableManager.GetTable(NameDict.section);
            var result = new List<SectionItem>(sectionTable.Rows.Count);
            startDate = startDate ?? DateTime.MinValue;
            foreach (DataRow row in sectionTable.Rows) {
                var newItem = createItem(row, shortInfo, startDate.Value, cityId);
                if (newItem.info.Count > 0)
                    result.Add(newItem);
            }
            return result;
        }

        public static SectionItem getByName(string name, DateTime? startDate = null, int? cityId = null) {
            SectionItem result = null;
            var sectionRow = getSecionRowByName(name);
            startDate = startDate ?? DateTime.MinValue;
            if (sectionRow != null)
                result = createItem(sectionRow, false, startDate.Value, cityId);
            else
                return null;
            return result.info.Any() ? result : null;
        }

        private static DataRow getSecionRowByName(string name) {
            var sectionRows = TableManager.SelectRowsFromTable(NameDict.section, NameDict.name, name);
            if (sectionRows.Length == 1) {
                return sectionRows[0];
            }
            return null;
        }

        public static void addNew(string name) {
            var row = TableManager.CreateRow(NameDict.section);
            row[NameDict.name] = name;
            TableManager.InsertRow(row);
        }

        public static bool saveOrUpdate(string sectionName, ConferenceItem changedItem) {
            var sectionRow = getSecionRowByName(sectionName);
            if (sectionRow != null) {
                var row = TableManager.SelectRowByPrimaryKey(NameDict.conference, changedItem.id);
                bool isNew = row == null;
                var newConference = ConferenceItem.saveOrUpdate(changedItem, row, sectionName);
                if (isNew) {
                    createLink(sectionRow[NameDict.id], newConference.id);
                }
                return true;
            }
            return false;
        }

        public static void createLink(object sectionId, object conferenceId) {
            var link = TableManager.CreateRow(NameDict.section_conference_link);
            link[NameDict.id] = TableManager.GetNewId(link.Table.TableName);
            link[NameDict.section_id] = sectionId;
            link[NameDict.conference_id] = conferenceId;
            TableManager.InsertRow(link);
        }

        private static SectionItem createItem(DataRow row, bool shortInfo, DateTime startDate, int? cityId = null) {
            var item = new SectionItem(row[NameDict.id], row[NameDict.name]);
            if (!shortInfo) {
                item.info = ConferenceItem.getBySection(row[NameDict.id], row[NameDict.name].ToString(), startDate, cityId);
            }
            return item;
        }
    }
}
