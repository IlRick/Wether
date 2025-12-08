using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wether.Model;

namespace Wether.Classes
{
    public class GetWether
    {
        public static string Url = "https://api.weather.yandex.ru/v2/forecast";
        public static string key = "demo_yandex_weather_api_key_ca6d09349ba0";

        public static async Task<DataResponse> Get(float lat, float lon)
        {
            DataResponse response = null;
            string url = $"{Url}?last={lat}&lon={lon}".Replace(",", ".");
            using (HttpClient Client = new HttpClient())
            {
                using (HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    Request.Headers.Add("X-Yandex-Weather-Key", key);
                    
                    using (var Response = await Client.SendAsync(Request))
                    {
                        string DataResponse = await Response.Content.ReadAsStringAsync();

                        response = JsonConvert.DeserializeObject<DataResponse>(DataResponse);
                    }

                }

            }
            return response;
        }   
    }
}




