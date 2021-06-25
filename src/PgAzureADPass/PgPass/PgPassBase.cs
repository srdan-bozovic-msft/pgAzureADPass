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
            string hostName,
            string database,
            int? port,
            string userName)
        {
            Name = name;
            HostName = hostName;
            Database = string.IsNullOrEmpty(database) ? "postgres" : database;
            Port = port.HasValue ? port.Value : 5432;
            UserName = userName;
        }

        public string Name { get; private set; }
        public string HostName { get; private set; }
        public string Server => HostName.Split('.').First();
        public string Database { get; private set; }
        public int Port { get; private set; }
        public string UserName { get; private set; }

        public string FileName => $"{Name}.pgpass";
        public long FileLength => 4096;

        public string GetContent()
        {
            var token = GetToken();
            var userName = string.IsNullOrEmpty(UserName) ? GetUserNameFromToken(token) : UserName;
            return $"{HostName}:{Port}:{Database}:{userName}:{token}";
        }


        protected abstract string GetToken();

        protected abstract string GetUserNameFromToken(string token);
    }
}
