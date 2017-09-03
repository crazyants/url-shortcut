using Microsoft.Extensions.Configuration;
using System.IO;
using Cassandra;

namespace URL_Shortcut.Utils
{
    public class CassandraConnect
    {
        private IConfigurationRoot configuration;

        private Cluster cluster;
        private ISession session;

        public CassandraConnect()
        {
            this.configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbsettings.json")
                .Build();

            this.cluster = null;
            this.session = null;

            this.Connect();
        }

        private ISession Connect()
        {
            string host = this.configuration["Cassandra:Host"];
            string port = this.configuration["Cassandra:Port"];
            string username = this.configuration["Cassandra:Username"];
            string password = this.configuration["Cassandra:Password"];
            string database = this.configuration["Cassandra:Database"];

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

            return null;
        }

        public ISession GetSession()
        {
            if (this.session == null)
            {
                this.Connect();
            }

            return this.session;
        }
    }
}
