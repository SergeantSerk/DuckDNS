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

            using var client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(uri);
            using HttpContent content = response.Content;
            return await content.ReadAsStringAsync();
        }
    }
}
