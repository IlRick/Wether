using Newtonsoft.Json;
using System;
using System.Net.Http;
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
            string json = await GetJson(lat, lon);
            return JsonConvert.DeserializeObject<DataResponse>(json);
        }
        public static async Task<string> GetJson(float lat, float lon)
        {
            // Исправлено: lat вместо last
            string url = $"{Url}?lat={lat}&lon={lon}".Replace(",", ".");

            using (HttpClient Client = new HttpClient())
            using (HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                Request.Headers.Add("X-Yandex-Weather-Key", key);

                using (var Response = await Client.SendAsync(Request))
                {
                    return await Response.Content.ReadAsStringAsync();
                }
            }
        }
        public static class GeoCoder
        {
            public static string Key = "67cd1532-2c21-4201-8586-c5e5ce8b3b56";

            public static async Task<(float lat, float lon)> GetCoords(string city)
            {
                string url =
                    $"https://geocode-maps.yandex.ru/1.x/?apikey={Key}&format=json&geocode={city}";

                using HttpClient client = new HttpClient();
                string json = await client.GetStringAsync(url);

                dynamic data = JsonConvert.DeserializeObject(json);

                string pos = data.response.GeoObjectCollection.featureMember[0]
                    .GeoObject.Point.pos;
                var parts = pos.Split(' ');

                float lon = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                float lat = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);

                return (lat, lon);
            }
        }

    }
}
