using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AzureADPgPass.PgPass
{
    class ActiveDirectoryManagedIdentityPgPass : PgPassBase
    {
        public ActiveDirectoryManagedIdentityPgPass(
            string name,
            string host,
            string database,
            int? port,
            string user,
            string clientId) : base(name, host, database, port, user)
        {
            ClientId = clientId;
        }

        public string ClientId { get; private set; }

        protected override string GetToken()
        {
            string miUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2Fossrdbms-aad.database.windows.net";
            if (ClientId != "")
            {
                miUrl += "&client_id=" + ClientId;
            }

            var request = (HttpWebRequest)WebRequest.Create(miUrl);
            request.Headers["Metadata"] = "true";
            request.Method = "GET";

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var streamResponse = new StreamReader(response.GetResponseStream());
                string stringResponse = streamResponse.ReadToEnd();
                var j = new JavaScriptSerializer();
                var list = (Dictionary<string, string>)j.Deserialize(stringResponse, typeof(Dictionary<string, string>));
                return list["access_token"];
            }
            catch (Exception e)
            {
                if(!string.IsNullOrEmpty(ClientId))
                {
                    return $"## User Assigned Managed Identity with client id={ClientId} not found ##";
                }
                else
                {
                    return $"## System Assigned Managed Identity not found ##";
                }
            }
        }
    }
}
