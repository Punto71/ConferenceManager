using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceManager.Service.Support;

namespace TestNupi.Service.Models {
    public class IdItem {

        public object id { get; set; }

        public IdItem(object _id) {
            id = _id;
        }
    }
}
