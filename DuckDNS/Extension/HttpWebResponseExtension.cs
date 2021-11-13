using System.Net;
using System.Threading.Tasks;

namespace DuckDNS.Extension
{
    public static class HttpWebResponseExt
    {
        public static async Task<WebResponse> GetResponseNoExceptionAsync(this WebRequest request)
        {
            try
            {
                return await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                if (e.Response is not WebResponse response)
                {
                    throw;
                }
                else
                {
                    return response;
                }
            }
        }
    }
}
