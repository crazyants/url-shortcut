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
            // Instantiate the Cassandra Counter class
            CassandraCounter cassandraCounter =
                new CassandraCounter("127.0.0.1", 9042, "url_shortcut", "urluser", "urluser");

            // Get the most recent count number from database
            long count = cassandraCounter.GetCounter();

            // Initialize the shared memory counter class with the count number
            SharedMemoryCounter.Initialize(count);

            // The port to which the server listens
            int port = 7079;

            // The maximum number of pending client connections
            int backlog = int.MaxValue;

            // Launch the server
            AsyncServerSocket.LaunchServer(port, backlog);
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
