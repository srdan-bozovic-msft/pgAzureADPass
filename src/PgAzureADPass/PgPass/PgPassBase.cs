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
            HostName = string.IsNullOrEmpty(hostName) ? "*" : hostName;
            Database = string.IsNullOrEmpty(database) ? "*" : database;
            Port = port.HasValue ? port.Value.ToString() : "*";
            UserName = string.IsNullOrEmpty(userName) ? "*" : userName;
        }

        public string Name { get; private set; }
        public string HostName { get; private set; }
        public string Database { get; private set; }
        public string Port { get; private set; }
        public string UserName { get; private set; }

        public string FileName => $"{Name}.pgpass";
        public long FileLength => 4096;

        public string GetContent()
        {
            return $"{HostName}:{Port}:{Database}:{UserName}:{GetToken()}";
        }

        protected abstract string GetToken();
   }
}
