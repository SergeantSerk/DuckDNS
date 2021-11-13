using System;
using System.Collections.Generic;
using System.Net;

namespace DuckDNS.Request
{
    public class AdvancedUpdateRequest : BaseUpdateRequest
    {
        public const string BaseUrl = "https://www.duckdns.org/update";

        public HashSet<string> Domains { get; set; }

        private IPAddress _IPv4;
        public IPAddress IPv4
        {
            get
            {
                return _IPv4;
            }
            set
            {
                if (value != null && value.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    throw new Exception("A valid IPv4 address must be supplied.");
                }
                else
                {
                    _IPv4 = value;
                }
            }
        }

        private IPAddress _IPv6;
        public IPAddress IPv6
        {
            get
            {
                return _IPv6;
            }
            set
            {
                if (value != null && value.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    throw new Exception("A valid IPv6 address must be supplied.");
                }
                else
                {
                    _IPv6 = value;
                }
            }
        }

        public bool? Verbose { get; set; } = null;

        public bool? Clear { get; set; } = null;

        public AdvancedUpdateRequest(Guid token, params string[] domains)
        {
            if (domains.Length < 1)
            {
                throw new Exception("At least one domain needs to be specified.");
            }

            Domains = new HashSet<string>(domains);
            Token = token;
            IPv4 = null;
            IPv6 = null;
        }

        // https://www.duckdns.org/update?domains={YOURVALUE}&token={YOURVALUE}[&ip={YOURVALUE}][&ipv6={YOURVALUE}][&verbose=true][&clear=true]
        public override string ToString()
        {
            string finalUrl = $"{BaseUrl}?domains={string.Join(',', Domains)}&token={Token}";
            if (IPv4 != null)
            {
                finalUrl += $"&ip={IPv4}";
            }
            if (IPv6 != null)
            {
                finalUrl += $"&ipv6={IPv6}";
            }
            if (Verbose != null)
            {
                finalUrl += $"&verbose={Verbose.ToString().ToLower()}";
            }
            if (Clear != null)
            {
                finalUrl += $"&clear={Clear.ToString().ToLower()}";
            }

            return finalUrl;
        }
    }
}
