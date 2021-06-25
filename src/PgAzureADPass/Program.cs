using AzureADPgPass.Configuration;
using AzureADPgPass.PgPass;
using DokanNet;
using DokanNet.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AzureADPgPass
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                if (!File.Exists("config.yml"))
                {
                    WriteUsage();
                    return;
                }
                args = new string[] { "config.yml" };
            }

            var path = args[0];

            Config config;
            if (!TryLoadConfig(path, out config))
            {
                Console.WriteLine($"Failed to load config {path}.");
                return;
            }
            if (!ConfigValidator.IsValidConfig(config))
            {
                Console.WriteLine($"Config is not valid {path}.");
                return;
            }

            Task.Run(() => MountVfs(config));

            Console.ReadKey();
        }

        private static void WriteUsage()
        {
            Console.WriteLine("Usage: aadpgpass [path-to-configuration]");
            Console.WriteLine();
            Console.WriteLine("path-to-configuration:");
            Console.WriteLine("    The path to an configuration .yml file.");
        }

        private static bool TryLoadConfig(string path, out Config config)
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var yml = File.ReadAllText(path);
                config = deserializer.Deserialize<Config>(yml);
                return true;
            }
            catch
            {
                config = null;
                return false;
            }

        }

        private static PgPassVfs CreateVfs(Config config)
        {
            var items = new List<PgPassBase>();
            foreach (var file in config.Files)
            {
                var authentication = file.Authentication;
                switch (authentication.ToLower())
                {
                    case "activedirectorymanagedidentity":
                        items.Add(
                            new ActiveDirectoryManagedIdentityPgPass(
                                file.Name, file.HostName, file.Database, file.Port, file.UserName, file.ClientId
                            ));
                        break;
                    case "activedirectoryinteractive":
                        items.Add(
                            new ActiveDirectoryInteractivePgPass(
                                file.Name, file.HostName, file.Database, file.Port, file.UserName, file.TenantId
                            ));
                        break;
                    case "activedirectoryintegrated":
                        items.Add(
                            new ActiveDirectoryIntegratedPgPass(
                                file.Name, file.HostName, file.Database, file.Port, file.UserName, file.TenantId
                            ));
                        break;
                    default:
                        break;
                }
            }
            return new PgPassVfs(config.VolumeLabel, items.ToArray());
        }

        private static void MountVfs(Config config)
        {
            try
            {
                Console.WriteLine($"Mounting {config.VolumeLabel} as drive {config.DriveLetter}:\\");
                var vfs = CreateVfs(config);
                vfs.Mount($"{config.DriveLetter}:\\",
                    DokanOptions.CurrentSession | 
                    DokanOptions.FixedDrive | 
                    DokanOptions.WriteProtection
                    , new NullLogger());
            }
            catch (DokanException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit ...");
            }
        }

    }
}
