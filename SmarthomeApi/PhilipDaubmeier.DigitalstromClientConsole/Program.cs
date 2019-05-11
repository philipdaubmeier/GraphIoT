using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using System;
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
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            Uri uri = ConsoleUtil.ReadUri("Enter DSS uri: (e.g. 'https://dss.local')");
            var username = ConsoleUtil.ReadNonWhitespace("Enter DSS username:");
            var password = ConsoleUtil.ReadPassword("Enter DSS password:");

            Func<X509Certificate2, bool> certValidation = (cert) => {
                Console.WriteLine($"Certificate fingerprint: {cert.GetCertHashString()}");
                Console.WriteLine("Trust this certificate? (y/n)");
                return Console.ReadLine().ToLowerInvariant().Trim().StartsWith('y');
            };
            var auth = new EphemeralDigitalstromAuth("ConsoleSample", username, password);
            var conn = new DigitalstromConnectionProvider(uri, auth, null, certValidation);
            using (var client = new DigitalstromDssClient(conn))
            {
                var zone = ConsoleUtil.ReadZone("Enter a room id (zone):");
                var group = ConsoleUtil.ReadGroup("Enter a color id:");
                var reachableScenes = await client.GetReachableScenes(zone, group);

                Console.WriteLine($"Reachable scenes in zone '{zone}', group '{group}':");
                Console.WriteLine(string.Join("\n", reachableScenes.ReachableScenes.Select(s => "\t" + s.ToDisplayString())));
            }
        }
    }
}