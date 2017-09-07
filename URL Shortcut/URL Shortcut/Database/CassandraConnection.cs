using Microsoft.Extensions.Configuration;
using System.IO;
using Cassandra;

namespace URL_Shortcut.Database
{
    public class CassandraConnection
    {
        // The configuration interface
        private IConfigurationRoot configuration;

        // The cluster and session variables
        private Cluster cluster;
        private ISession session;

        public CassandraConnection()
        {
            // Read the Cassandra config file
            this.configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbsettings.json")
                .Build();

            // Initiate the attributes
            this.cluster = null;
            this.session = null;

            // Get connected to the database
            this.Connect();
        }

        private void Connect()
        {
            // Get connection settings
            string host = this.configuration["Cassandra:Host"];
            string port = this.configuration["Cassandra:Port"];
            string username = this.configuration["Cassandra:Username"];
            string password = this.configuration["Cassandra:Password"];
            string database = this.configuration["Cassandra:KeySpace"];

            // Build the connection
            this.cluster = Cluster
                .Builder()
                .AddContactPoint(host)
                .WithPort(int.Parse(port))
                .WithCredentials(username, password)
                .WithDefaultKeyspace(database)
                .WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.All))
                .WithLoadBalancingPolicy(new RoundRobinPolicy())
                .WithCompression(CompressionType.Snappy)
                .Build();

            // Get the session
            this.session = this.cluster.Connect();
        }

        public ISession GetSession()
        {
            // Reconnect if not
            if (this.session == null)
            {
                this.Connect();
            }

            // Return the session
            return this.session;
        }
    }
}
