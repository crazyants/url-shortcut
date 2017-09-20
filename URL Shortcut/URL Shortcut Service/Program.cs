using System.ServiceProcess;

namespace URL_Shortcut_Service
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            // Debug mode
            ListenerService listenerService;
            listenerService = new ListenerService();
            listenerService.GoDebug(null);
#else
            // Normal mode: Run the service
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
