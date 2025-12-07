using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Wether.Classes
{
    public class GetWether
    {
        public static string url = "https://api.weather.yandex.ru/v2/forecast";
        public static string key = "demo_yandex_weather_api_key_ca6d09349ba0";

        public static async string Get(float lat, float lon)
        {
            using (HttpClient Client = new HttpClient())
            {
                using (HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Get, $"{url}?last={lat}&lon={lon}".Replace(",", ".")))
                {
                    Request.Headers.Add("X-Yandex-Weather-Key", key);
                    using (var Response = await Client.SendAsync(Request))
                    {
                        string DataResponse = await Response.Content.ReadAsStringAsync();
                    }
                }
                    
            }
        }
    }
}




