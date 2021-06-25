using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureADPgPass.PgPass
{
    abstract class PgPassBase
    {
        public PgPassBase(
            string name,
            string host,
            string database,
            int? port)
        {
            Name = name;
            Host = host;
            Database = string.IsNullOrEmpty(database) ? "postgres" : database;
            Port = port.HasValue ? port.Value : 5432;
        }

        public string Name { get; private set; }
        public string Host { get; private set; }
        public string Server => Host.Split('.').First();
        public string Database { get; private set; }
        public int Port { get; private set; }

        public string FileName => $"{Name}.pgpass";
        public long FileLength => 4096;

        public string GetContent()
        {
            var token = GetToken();
            var user = GetUser(token);
            return $"{Host}:{Port}:{Database}:{user}:{token}";
        }


        protected abstract string GetToken();

        protected abstract string GetUser(string token);
    }
}
