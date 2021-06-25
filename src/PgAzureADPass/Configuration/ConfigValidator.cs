using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureADPgPass.Configuration
{
    class ConfigValidator
    {
        public static bool IsValidConfig(Config config)
        {
            if (!ValidateDriveLetter(config))
            {
                Console.WriteLine($"Invalid drive letter {config.DriveLetter}.");
                return false;
            }
            if (!ValidateFiles(config))
            {
                return false;
            }
            return true;
        }

        private static bool ValidateDriveLetter(Config config)
        {
            var regex = new Regex("^[a-zA-Z]$");
            return regex.IsMatch(config.DriveLetter);
        }

        private static bool ValidateFiles(Config config)
        {
            if (config.Files == null || config.Files.Length == 0)
            {
                Console.WriteLine("No files.");
                return false;
            }
            foreach (var file in config.Files)
            {
                if (!ValidateFile(file))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateFile(Config.PgPass file)
        {
            var name = file.Name;
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name is required property.");
                return false;
            }

            var hostName = file.HostName;
            if (string.IsNullOrEmpty(hostName))
            {
                Console.WriteLine($"{name}: HostName is required property.");
                return false;
            }

            var authentication = file.Authentication;
            if (string.IsNullOrEmpty(authentication))
            {
                Console.WriteLine($"{name}: Authentication is required property.");
                return false;
            }

            switch (authentication.ToLower())
            {
                case "activedirectorymanagedidentity":
                    if(!ValidateActiveDirectoryManagedIdentity(file))
                    {
                        return false;
                    }
                    break;
                case "activedirectoryinteractive":
                    if (!ValidateActiveDirectoryInteractive(file))
                    {
                        return false;
                    }
                    break;
                case "activedirectoryintegrated":
                    if (!ValidateActiveDirectoryIntegrated(file))
                    {
                        return false;
                    }
                    break;
                default:
                    Console.WriteLine($"{name}: Authentication type not supported.");
                    return false;
            }

            return true;
        }

        private static bool ValidateActiveDirectoryManagedIdentity(Config.PgPass file)
        {
            var name = file.Name;

            var userName = file.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine($"{name} UserName is required property.");
                return false;
            }

            return true;
        }

        private static bool ValidateActiveDirectoryInteractive(Config.PgPass file)
        {
            var name = file.Name;

            var tenantId = file.TenantId;
            if (string.IsNullOrEmpty(tenantId))
            {
                Console.WriteLine($"{name} TenantId is required property.");
                return false;
            }

            return true;
        }

        private static bool ValidateActiveDirectoryIntegrated(Config.PgPass file)
        {
            var name = file.Name;

            var tenantId = file.TenantId;
            if (string.IsNullOrEmpty(tenantId))
            {
                Console.WriteLine($"{name} TenantId is required property.");
                return false;
            }

            return true;
        }
    }
}
