using CommandLine;
using DuckDNS.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

namespace DuckDNS.Updater
{
    public class CommandLineOptions
    {
        [Option('t', "token", Required = true, HelpText = "Token used to update domain from an account.")]
        public Guid Token { get; set; }

        [Option('d', "domains", Required = true, HelpText = "Domains to update, seperated as a comma-separated value.")]
        public IEnumerable<string> Domains { get; set; }

        [Option('i', "interval", Required = false, HelpText = "The period, in non-negative seconds, to refresh and update IPv(4|6) address. 0 seconds is one-shot execution.", Min = 0)]
        public int Interval { get; set; }

        [Option("ipv4", Default = false, Required = false, HelpText = "Update IPv4 address only.")]
        public bool IPv4Only { get; set; }

        [Option("ipv6", Default = false, Required = false, HelpText = "Update IPv6 address only.")]
        public bool IPv6Only { get; set; }

        [Option('v', "verbose", Default = false, Required = false, HelpText = "Display extra details about the updates.")]
        public bool Verbose { get; set; }
    }

    public static class Program
    {
        public static readonly string[] IPv4Resolvers = new string[]
        {
            "https://ipv4.icanhazip.com",
            "https://api.ipify.org",
            "https://ipv4.wtfismyip.com/text"
        };

        public static readonly string[] IPv6Resolvers = new string[]
        {
            "https://ipv6.icanhazip.com",
            "https://api6.ipify.org",
            "https://ipv6.wtfismyip.com/text"
        };

        // -t = token (required)
        // -4 = IPv4 only
        // -6 = IPv6 only
        // -v = verbose
        // -d = domains (required, comma-separated values)
        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsedAsync(RunOptionsAsync);
        }

        private static async Task RunOptionsAsync(CommandLineOptions arg)
        {
            var timer = new Timer(arg.Interval * 1000)
            {
                AutoReset = arg.Interval != 0,
                Enabled = true
            };
            Console.WriteLine($"Set up and enabled timer with {arg.Interval}-second intervals and auto-reset {(timer.AutoReset ? "enabled" : "disabled")}.");

            timer.Elapsed += async (o, e) =>
            {
                Console.WriteLine("Timer triggered, beginning update...");
                try
                {
                    IPAddress ipv4Address = null, ipv6Address = null;

                    // Condition: Both -4 and -6 or neither are specified
                    bool bothOrNeither = (arg.IPv4Only && arg.IPv6Only) || (!arg.IPv4Only && !arg.IPv6Only);
                    if (bothOrNeither || (arg.IPv4Only && !arg.IPv6Only))
                    {
                        // Handle IPv4
                        
                        Console.WriteLine(ipv4Address = await GetIPAddressAsync(AddressFamily.InterNetwork));
                    }

                    if (bothOrNeither || (arg.IPv6Only && !arg.IPv4Only))
                    {
                        // Handle IPv6
                        Console.Write("Acquiring IPv6 address...");
                        Console.WriteLine(ipv6Address = await GetIPAddressAsync(AddressFamily.InterNetworkV6));
                    }

                    var duckDnsApi = new DuckDnsApi(arg.Token);
                    foreach (var domain in arg.Domains)
                    {
                        await UpdateAddress(duckDnsApi, ipv4Address, arg.Verbose, domain);
                        await UpdateAddress(duckDnsApi, ipv6Address, arg.Verbose, domain);
                    }
                }
                finally
                {
                    Console.WriteLine("Encountered an error while running the timer, cleaning up...");
                    timer.Dispose();
                }
            };

            await Task.Delay(-1);
            timer.Dispose();    // If it ever gets here
        }

        private static async Task<IPAddress> GetIPAddressAsync(AddressFamily addressFamily)
        {
            // TO-DO: choose random provider each time, build consensus or let user choose provider
            string result = addressFamily switch
            {
                AddressFamily.InterNetwork => await RequestHandler.GetAsync(IPv4Resolvers[0]),
                AddressFamily.InterNetworkV6 => await RequestHandler.GetAsync(IPv6Resolvers[0]),
                _ => throw new Exception($"Address family must be either IPv4 ({nameof(AddressFamily.InterNetwork)}) or IPv6 ({nameof(AddressFamily.InterNetworkV6)})."),
            };
            return IPAddress.Parse(result.Trim('\n', '\r'));
        }

        private static async Task UpdateAddress(DuckDnsApi duckDnsApi, IPAddress address, bool verbose, string domain)
        {
            if (address == null)
            {
                return;
            }

            Console.Write($"Updating domain \"{string.Join(", ", domain)}\" to {address}...");
            string result = await duckDnsApi.UpdateAsync(
                ipv4: address.AddressFamily == AddressFamily.InterNetwork ? address : null,
                ipv6: address.AddressFamily == AddressFamily.InterNetworkV6 ? address : null,
                verbose: verbose,
                domains: domain);
            Console.WriteLine(result);
        }
    }
}
