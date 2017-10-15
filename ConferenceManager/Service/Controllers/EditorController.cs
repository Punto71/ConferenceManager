using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ConferenceManager.Service.Database;
using ConferenceManager.Service.Support;
using TestNupi.Service.Models;
using TestNupi.Service.Models.Editors;

namespace TestNupi.Service.Controllers {
    [RoutePrefix("editor")]
    public class EditorController : BaseController {

        /// <summary>
        /// Создание пустого элемента
        /// </summary>
        [Route("new/{entity}")]
        [HttpGet]
        public HttpResponseMessage createNew(string entity) {
            DataRow row = null;
            var table = TableManager.GetTable(entity.ToUpper());
            if (table != null)
                row = table.NewRow();
            return prepareResponce(row);
        }

        /// <summary>
        /// Редактирование существующиего элемента
        /// </summary>
        [Route("edit/{entity}/{id}")]
        [HttpGet]
        public HttpResponseMessage createNew(string entity, int id) {
            var row = TableManager.SelectRowByPrimaryKey(entity.ToUpper(), id);
            return prepareResponce(row);
        }

        /// <summary>
        /// Сохранение элемента
        /// </summary>
        [Route("save/{entity}")]
        [HttpPut]
        public HttpResponseMessage save(string entity) {
            var tableName = entity.ToUpper();
            var body = Request.Content.ReadAsStringAsync().Result;
            var text = HttpUtility.UrlDecode(body);
            var table = TableManager.GetTable(tableName);
            text = text.IndexOf("data=") == 0 ? text.Remove(0, 5) : text;
            var obj = JsonConvert.DeserializeObject(text) as JObject;
            if (obj != null) {
                var newRow = parseResponse(obj, table.NewRow());
                var isNew = false;
                var existId = tryGetExistRowId(newRow);
                if (existId.HasValue)
                    newRow[NameDict.id] = existId.Value;
                if (isNew = newRow[NameDict.id] == DBNull.Value)
                    newRow[NameDict.id] = TableManager.GetNewId(tableName);
                var id = newRow[NameDict.id];
                string message = null;
                try {
                    if (isNew)
                        TableManager.InsertRow(newRow);
                    else
                        TableManager.UpdateRecord(TableManager.SelectRowByPrimaryKey(tableName, id), newRow);
                    message = afterSave(newRow, obj, isNew);
                } catch (FbException ex) {
                    if (ex.ErrorCode == 335544349)
                        message = "Нарушение уникальности";
                }
                if (message != null)
                    return createResponse(HttpStatusCode.Conflict, message);
                return createResponse(HttpStatusCode.OK, id);
            }
            return prepareResponce(null);
        }

        private static int? tryGetExistRowId(DataRow newRow) {
            var tableName = newRow.Table.TableName;
            try {
                switch (tableName) {
                    case NameDict.member:
                        return (int)TableManager.SelectScalarFromTable(tableName, NameDict.email, newRow[NameDict.email], NameDict.id);
                }
            } catch (Exception ex) { }
            return null;
        }

        private static string afterSave(DataRow savedRow, JObject savedObject, bool isNewItem) {
            switch (savedRow.Table.TableName) {
                case NameDict.conference:
                    var sectionId = savedObject[NameDict.section_id];
                    if (isNewItem && sectionId != null && sectionId.Type == JTokenType.Integer && (int)sectionId > 0)
                        SectionItem.createLink(savedObject[NameDict.section_id], savedRow[NameDict.id]);
                    break;
                case NameDict.member:
                    var conferenceId = savedObject[NameDict.conference_id];
                    if (conferenceId != null && conferenceId.Type == JTokenType.Integer && (int)conferenceId > 0) {
                        var query = string.Format("{0} = {2} AND {1} = {3}", NameDict.member_id, NameDict.conference_id, savedRow[NameDict.id], conferenceId);
                        if (TableManager.GetTable(NameDict.member_conference_link).Select(query).Length == 0)
                            createMemberLink(conferenceId, savedRow[NameDict.id]);
                        else
                            return "Вы уже зарегистрированы в этой конференции";
                    }
                    break;
            }
            return null;
        }

        private static void createMemberLink(object conferenceId, object memberId) {
            var link = TableManager.CreateRow(NameDict.member_conference_link);
            link[NameDict.id] = TableManager.GetNewId(link.Table.TableName);
            link[NameDict.member_id] = memberId;
            link[NameDict.conference_id] = conferenceId;
            TableManager.InsertRow(link);
        }

        private static DataRow parseResponse(JObject obj, DataRow row) {
            var startIndexColumns = new SortedDictionary<int, string>();
            foreach (DataColumn col in row.Table.Columns) {
                if (obj[col.ColumnName] != null) {
                    var type = col.DataType;
                    var value = obj[col.ColumnName];
                    if (value.Type != JTokenType.Null)
                        row[col] = Convert.ChangeType(value, type);
                }

            }
            return row;
        }

        private HttpResponseMessage prepareResponce(DataRow row) {
            if (row != null)
                return createResponse(HttpStatusCode.OK, EditorItem.createByDataRow(row));
            else
                return createResponse(HttpStatusCode.NotFound);
        }
    }
}
