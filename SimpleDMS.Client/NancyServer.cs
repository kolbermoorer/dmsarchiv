using Nancy;
using Nancy.Hosting.Self;
using SimpleDMS.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDMS.Database;

namespace SimpleDMS.Client
{
    class NancyServer
    {
        private NancyHost hostNancy;
        private string hostUrl;

        public void Start()
        {
            if (_isArchivedInstalled("DMSArchiv"))
            {
                DMS.Session.Init("DMSArchiv");
            }
            else
            {
                Connection conn = new Connection(SimpleServerExtensionMethods.GetDataPath("DMSArchiv"), "DMSArchiv");
                Start();
            }

            var hostConfig = new HostConfiguration
            {
                UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true
                },
            };

            if (hostUrl == null) hostUrl = "http://localhost:8089/";

            hostNancy = new NancyHost(hostConfig, new Uri(hostUrl));
            hostNancy.Start();

        }

        /// <summary>
        /// Check If Archive Database Exists or Not
        /// </summary>
        /// <returns></returns>
        private bool _isArchivedInstalled(string archiveName)
        {
            string rootPath = Path.Combine(SimpleServerExtensionMethods.GetDataPath(archiveName), "db-" + archiveName, "DMSArchiv.sqlite");
            return File.Exists(rootPath);
        }

        public void Stop()
        {
            hostNancy.Stop();
        }
    }

    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new NancyCustomRootPathProvider(); }
        }
    }

    public class NancyCustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
#if DEBUG
            return @"D:\Projekte\Privat\SimpleDMS\SimpleDMS.Client";
#else
        return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif
        }
    }
}
