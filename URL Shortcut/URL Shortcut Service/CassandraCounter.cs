using Cassandra;
using System.Linq;

namespace URL_Shortcut_Service
{
    class CassandraCounter
    {
        // The database cluster and connection session objects
        private Cluster cluster;
        private ISession session;

        // Connection parameters
        private string host;
        private int port;
        private string database;
        private string username;
        private string password;

        public CassandraCounter(string host, int port, string database, string username, string password)
        {
            // Will be initiated on request
            this.cluster = null;
            this.session = null;

            this.host = host;
            this.port = port;
            this.database = database;
            this.username = username;
            this.password = password;
        }

        public long GetCounter()
        {
            // Connect to the database
            this.Connect();

            long value = -1;

            var key = "n";
            var cql = string.Format("SELECT counter AS {0} FROM tbl_counters WHERE key = ? ;", key);
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind("urls");
            var rows = this.session.Execute(stmt);

            var row = rows.GetRows().ToList()[0];

            value = row.GetValue<long>(key);

            // Disconnect from the database
            this.Disconnect();

            return value;
        }

        private void Connect()
        {
            // Establish the connection *with consistency level of ALL*
            this.cluster = Cluster
                .Builder()
                .AddContactPoint(this.host)
                .WithPort(this.port)
                .WithCredentials(this.username, this.password)
                .WithDefaultKeyspace(this.database)
                .WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.All))
                .WithLoadBalancingPolicy(new RoundRobinPolicy())
                .WithCompression(CompressionType.NoCompression)
                .Build();

            // Get the session
            this.session = this.cluster.Connect();
        }

        private void Disconnect()
        {
            // Destroy the established connection
            this.cluster.Shutdown();
        }
    }
}
