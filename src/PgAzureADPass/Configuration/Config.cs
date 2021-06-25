using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureADPgPass.Configuration
{
    class Config
    {
        public string VolumeLabel { get; set; }
        public string DriveLetter { get; set; }

        public PgPass[] Files { get; set; }
        public class PgPass
        {
            public string Name { get; set; }
            public string Authentication { get; set; }
            public string HostName { get; set; }
            public string Database { get; set; }
            public int? Port { get; set; }
            public string UserName { get; set; }
            public string ClientId { get; set; }
            public string TenantId { get; set; }
        }
    }
}
