using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DuckDNS
{
    public static class RequestHandler
    {
        public static async Task<string> GetAsync(string uri)
        {
            /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            using HttpWebResponse response = (HttpWebResponse)await request.GetResponseNoExceptionAsync();
            using Stream stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();*/

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            using WebResponse response = await request.GetResponseAsync();
            using Stream stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
