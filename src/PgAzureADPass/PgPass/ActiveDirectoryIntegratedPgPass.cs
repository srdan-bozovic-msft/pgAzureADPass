using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureADPgPass.PgPass
{
    class ActiveDirectoryIntegratedPgPass : ActiveDirectoryPgPassBase
    {
        public ActiveDirectoryIntegratedPgPass(
            string name,
            string hostName,
            string database,
            int? port,
            string userName,
            string tenantId) : base(name, hostName, database, port, userName, tenantId)
        {

        }

        protected override Task<AuthenticationResult> AzureADAcquireTokenAsync(IEnumerable<string> scopes)
        {
            return App.AcquireTokenByIntegratedWindowsAuth(scopes).ExecuteAsync();
        }
    }
}
