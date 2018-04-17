using System.Web.Http;
using Owin;
using System.IO;
using System.Reflection;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using System.Diagnostics;

namespace ConferenceManager.Service {
    class ServerConfig {
        public void Configuration(IAppBuilder app) {
            app.UseWebApi(configureWebApi());
            app.UseFileServer(configureStaticFiles());
        }

        private HttpConfiguration configureWebApi() {
            var config = new HttpConfiguration();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MapHttpAttributeRoutes();
            return config;
        }

        private FileServerOptions configureStaticFiles() {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Client");
            var physicalFileSystem = new PhysicalFileSystem(path);
            var options = new FileServerOptions {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            if (Debugger.IsAttached) {
                options.EnableDirectoryBrowsing = true;
            }
            options.DefaultFilesOptions.DefaultFileNames = new[] { "home.html" };
            return options;
        }
    }
}
