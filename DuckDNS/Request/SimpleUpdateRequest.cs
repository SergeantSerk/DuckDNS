using System;
using System.Net;

namespace DuckDNS.Request
{
    public class SimpleUpdateRequest : BaseUpdateRequest
    {
        public const string BaseUrl = "https://duckdns.org/update";

        public string Domain { get; set; }

        public IPAddress IP { get; set; } = null;

        public SimpleUpdateRequest(Guid token, string domain)
        {
            Token = token;
            Domain = domain;
        }

        // https://duckdns.org/update/{YOURDOMAIN}/{YOURTOKEN}[/{YOURIPADDRESS}]
        public override string ToString()
        {
            return $"{BaseUrl}/{Domain}/{Token}/{(IP?.ToString() ?? "")}";
        }
    }
}
