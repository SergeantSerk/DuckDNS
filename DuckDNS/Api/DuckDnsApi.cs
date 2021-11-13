using DuckDNS.Request;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DuckDNS.Api
{
    public class DuckDnsApi
    {
        public Guid Token { get; set; }

        public DuckDnsApi(Guid token)
        {
            Token = token;
        }

        public async Task<string> UpdateAsync(string domain)
        {
            return await UpdateAsync(domain, null);
        }

        public async Task<string> UpdateAsync(string domain, IPAddress ip)
        {
            var request = new SimpleUpdateRequest(Token, domain)
            {
                IP = ip
            };
            return await RequestHandler.GetAsync(request.ToString());
        }

        public async Task<string> UpdateAsync(IPAddress ipv4 = null, IPAddress ipv6 = null, bool? verbose = null, bool? clear = null, params string[] domains)
        {
            var request = new AdvancedUpdateRequest(Token, domains)
            {
                IPv4 = ipv4,
                IPv6 = ipv6,
                Verbose = verbose,
                Clear = clear
            };
            return await RequestHandler.GetAsync(request.ToString());
        }

        public async Task<string> ClearAsync(bool? verbose = null, params string[] domains)
        {
            var request = new AdvancedUpdateRequest(Token, domains)
            {
                Clear = true,
                Verbose = verbose
            };
            return await RequestHandler.GetAsync(request.ToString());
        }
    }
}
