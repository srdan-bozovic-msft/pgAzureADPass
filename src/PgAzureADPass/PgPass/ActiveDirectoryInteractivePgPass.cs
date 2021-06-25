using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureADPgPass.PgPass
{
    class ActiveDirectoryInteractivePgPass : ActiveDirectoryPgPassBase
    {
        public ActiveDirectoryInteractivePgPass(
            string name,
            string host,
            string database,
            int? port,
            string tenantId) : base(name, host, database, port, tenantId)
        {

        }

        protected override Task<AuthenticationResult> AzureADAcquireTokenAsync(IEnumerable<string> scopes)
        {
            return App.AcquireTokenInteractive(scopes).ExecuteAsync();
        }
    }
}
