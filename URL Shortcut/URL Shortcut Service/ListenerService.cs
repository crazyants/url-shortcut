using System.ServiceProcess;

namespace URL_Shortcut_Service
{
    public partial class ListenerService : ServiceBase
    {
        public ListenerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // The port to which the server listens
            int port = 7079;

            // The maximum number of pending client connections
            int backlog = int.MaxValue;

            // Launch the server
            AsyncSocketServer.LaunchServer(port, backlog);
        }

        protected override void OnStop()
        {
        }

        // An entry point for debugging purposes
        public void GoDebug(string[] args)
        {
            this.OnStart(args);
        }
    }
}
