﻿namespace URL_Shortcut_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            ListenerService listenerService;
            listenerService = new ListenerService();
            listenerService.GoDebug(null);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ListenerService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
