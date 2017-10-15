using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ConferenceManager.Service.Support;
using TestNupi.Service.Models;

namespace TestNupi.Service.Controllers {
    public class DictController : ApiController {

        /// <summary>
        /// Получение всех городов
        /// </summary>
        [Route("city")]
        [HttpGet]
        public List<CityItem> getCityList() {
            return CityItem.getAll();
        }

        /// <summary>
        /// Получение всех городов, в которых проходят конференции
        /// </summary>
        [Route("city/used")]
        [HttpGet]
        public List<CityItem> getUsedCity() {
            var list = CityItem.getUsed(DateTime.Now.Date);
            list.Insert(0,new CityItem(NameDict.all_item_id,NameDict.all_city,null));
            return list;
        }

        /// <summary>
        /// Получение всех областей
        /// </summary>
        [Route("region")]
        [HttpGet]
        public List<IdValueItem> getRegionList() {
            return IdValueItem.getAll(NameDict.region, NameDict.name);
        }

        /// <summary>
        /// Получение всех областей
        /// </summary>
        [Route("section")]
        [HttpGet]
        public List<IdValueItem> getSectionList() {
            var list = IdValueItem.getAll(NameDict.section, NameDict.name);
            list.Insert(0, new IdValueItem(NameDict.all_item_id, NameDict.all_section));
            return list;
        }

    }
}
