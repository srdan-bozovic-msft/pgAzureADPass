using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureADPgPass.PgPass
{
    abstract class ActiveDirectoryPgPassBase : PgPassBase
    {
        public ActiveDirectoryPgPassBase(
            string name,
            string host,
            string database,
            int? port,
            string tenantId) : base(name, host, database, port)
        {
            var clientId = "1950a258-227b-4e31-a9cf-717495945fc2";
            App = PublicClientApplicationBuilder.Create(clientId)
                .WithTenantId(tenantId)
                .Build();
        }

        private static AutoResetEvent AutoResetEvent = new AutoResetEvent(true);

        public IPublicClientApplication App { get; private set; }

        private async Task<AuthenticationResult> AcquireTokenCommon(IEnumerable<String> scopes)
        {
            AuthenticationResult result = null;
            var accounts = await App.GetAccountsAsync();

            if (accounts.Any())
            {
                try
                {
                    result = await App.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
                }
                catch (MsalUiRequiredException)
                {
                }
            }

            if (result == null)
            {
                result = await AzureADAcquireTokenAsync(scopes);
            }
            return result;
        }

        protected abstract Task<AuthenticationResult> AzureADAcquireTokenAsync(IEnumerable<string> scopes);

        protected override string GetToken()
        {
            try
            {
                AutoResetEvent.WaitOne();
                var result = AcquireTokenCommon(
                    new[] { "https://ossrdbms-aad.database.windows.net/.default" }
                    ).GetAwaiter().GetResult();
                return result.AccessToken;
            }
            catch (Exception e)
            {
                return $"## Failed to acquire token: {e.Message} ##";
            }
            finally
            {
                AutoResetEvent.Set();
            }
        }

        protected override string GetUser(string token)
        {
            try
            {
                var jwt = new JwtSecurityToken(token);
                var upn = jwt.Payload.Claims.FirstOrDefault(c => c.Type == "upn")?.Value;
                return $"{upn}@{Server}";
            }
            catch (Exception e)
            {
                return $"## User not found ##";
            }
        }
    }
}
