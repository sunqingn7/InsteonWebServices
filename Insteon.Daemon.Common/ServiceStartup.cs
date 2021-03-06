﻿using System;
using System.Configuration;
using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;

namespace Insteon.Daemon.Common
{
    public static class ServiceStartup
    {
        static InsteonAppListenerHost appHost;

        public static string ListeningOn => ConfigurationManager.AppSettings["listenOn"];

        public static void Main()
        {
#if DEBUG
            LogManager.LogFactory = new ConsoleLogFactory();
#else
            LogManager.LogFactory = new NLogFactory();
#endif
            var logger = LogManager.GetLogger(typeof(ServiceStartup));

            try
            {
                var insteonConnection = ConfigurationManager.AppSettings["insteonConnection"];

                appHost = new InsteonAppListenerHost(insteonConnection);

                appHost.Init();
                appHost.Start(ListeningOn);
                logger.InfoFormat("Listening On: {0}", ListeningOn);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("{0}: {1}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public static void Stop()
        {
            appHost?.Stop();
        }

        public static AppHostHttpListenerBase GetAppHostListner()
        {
            LogManager.LogFactory = new NLogFactory();
            var insteonConnection = ConfigurationManager.AppSettings["insteonConnection"];
            return new InsteonAppListenerHost(insteonConnection);
        }

    }
}
