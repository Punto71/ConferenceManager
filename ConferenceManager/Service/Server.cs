using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Owin.Hosting;

namespace ConferenceManager.Service {
    public class Server {

        public static void start(int port, string ip = "*") {
            var baseAddress = string.Format("http://{0}:{1}", ip, port);
            Logger.addInfo("Сервер запущен. Адрес: " + baseAddress);
            WebApp.Start<ServerConfig>(url: baseAddress);
        }
    }
}
