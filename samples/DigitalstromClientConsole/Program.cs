using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in main: {ex.Message}");
                Console.WriteLine($"Exiting program.");
            }
        }

        static async Task MainAsync(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            Uri uri = ConsoleUtil.ReadUri("Enter DSS uri: (e.g. 'https://dss.local:8080')");

            static IDigitalstromAuth loginUser()
            {
                var username = ConsoleUtil.ReadNonWhitespace("Enter DSS username:");
                var password = ConsoleUtil.ReadPassword("Enter DSS password:");
                return new EphemeralDigitalstromAuth("ConsoleSample", username, password);
            }

            static bool validateCert(X509Certificate2 cert)
            {
                Console.WriteLine($"Certificate fingerprint: {cert.GetCertHashString()}");
                Console.WriteLine("Trust this certificate? (y/n)");
                var read = Console.ReadLine();
                return (read ?? string.Empty).ToLowerInvariant().Trim().StartsWith('y');
            }

            using var conn = new DigitalstromConnectionProvider(uri, loginUser, validateCert);

            var client = new DigitalstromDssClient(conn);
            var zone = ConsoleUtil.ReadZone("Enter a room id (zone):");
            var group = ConsoleUtil.ReadGroup("Enter a color id:");
            var reachableScenes = await client.GetReachableScenes(zone, group);

            Console.WriteLine($"Reachable scenes in zone '{zone}', group '{group}':");
            Console.WriteLine(string.Join("\n", (reachableScenes.ReachableScenes ?? new List<Scene>()).Select(s => "\t" + s.ToString("d", CultureInfo.CurrentUICulture))));
        }
    }
}