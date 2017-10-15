using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ConferenceManager.Service.Support;
using TestNupi.Service.Models;

namespace TestNupi.Service.Controllers {
    [RoutePrefix("conference")]
    public class ConferenceController : BaseController {

        const string sectionRoute = "{section}/info";

        /// <summary>
        /// Получение информации по всем секциям и конференциям
        /// </summary>
        [Route("info")]
        [HttpGet]
        public HttpResponseMessage getAllConferenceInfo(int? city = null) {
            if (city < 1)
                city = null;
            var result = SectionItem.getFull(false, DateTime.Now.Date, city);
            if (result.Any())
                return createResponse(HttpStatusCode.OK, result);
            else
                return createResponse(HttpStatusCode.NotFound, getNotFoundMessage());
        }

        /// <summary>
        /// Получение списка конференций по конкретной секции
        /// </summary>
        [Route(sectionRoute)]
        [HttpGet]
        public HttpResponseMessage getSectionInfo(string section, int? city = null) {
            var item = SectionItem.getByName(section, DateTime.Now.Date, city);
            if (item == null)
                return createResponse(HttpStatusCode.NotFound, getNotFoundMessage(section));
            else
                return createResponse(HttpStatusCode.OK, item);
        }

        private static string getNotFoundMessage(string sectionName = null) {
            if (sectionName == null)
                return "На текущую дату нет актуальных конференций";
            else
                return "На текущую дату для секции " + sectionName + " нет актуальных конференций";
        }
    }
}
