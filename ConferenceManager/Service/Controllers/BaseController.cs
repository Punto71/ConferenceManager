using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace TestNupi.Service.Controllers {
    public class BaseController : ApiController {

        protected HttpResponseMessage createResponse(HttpStatusCode code, object item = null) {
            if (item == null)
                return ControllerContext.Request.CreateResponse(code);
            else
                return ControllerContext.Request.CreateResponse(code, item);
        }

        protected HttpResponseMessage createResponse(object item) {
            if (item == null)
                return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);
            else
                return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, item);
        }
    }
}
