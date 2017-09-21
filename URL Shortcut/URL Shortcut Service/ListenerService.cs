using System.ServiceProcess;
using System.Threading;

namespace URL_Shortcut_Service
{
    public partial class ListenerService : ServiceBase
    {
        private const int THREAD_TIMEOUT_15SEC = 15000;

        // The thread that runs the listener
        private Thread thread;

        // The flag that terminates the listener thread
        private bool running;

        public ListenerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Initialize the thread
            this.thread = new Thread(new ThreadStart(RunListener));

            // Initialize the flag by setting it as running
            this.running = true;

            // Start the thread
            this.thread.Start();
        }

        protected override void OnStop()
        {
            // Stop the thread by setting off the flag
            this.running = false;

            // Wait until thread finishes
            this.thread.Join(THREAD_TIMEOUT_15SEC);

            // Terminate the thread
            if (this.thread.IsAlive)
            {
                this.thread.Abort();
            }
        }

        // An entry point for debugging purposes
        public void GoDebug(string[] args)
        {
            this.OnStart(args);
        }

        private void RunListener()
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
            AsyncServerSocket.LaunchServer(port, backlog, ref this.running);
        }
    }
}
