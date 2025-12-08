using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wether.Classes
{
    public static class CacheService
    {
        private static string connectionString =
            "Server=localhost;Database=wether;Uid=root;Pwd=;Charset=utf8;";

        public static bool TryGetWeather(string city, out string json)
        {
            json = null;

            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query = "SELECT json, last_update FROM weather_cache WHERE city=@city";

            using var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@city", city);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return false;

            string savedJson = reader.GetString("json");
            DateTime lastUpdate = reader.GetDateTime("last_update");

            if ((DateTime.Now - lastUpdate).TotalMinutes < 30)
            {
                json = savedJson;
                return true;
            }

            return false;
        }

        public static void SaveWeather(string city, string json)
        {
            using var con = new MySqlConnection(connectionString);
            con.Open();

            string query =
                @"INSERT INTO weather_cache (city, json, last_update)
                  VALUES (@city, @json, @time)
                  ON DUPLICATE KEY UPDATE 
                      json=@json, 
                      last_update=@time";

            using var cmd = new MySqlCommand(query, con);

            cmd.Parameters.AddWithValue("@city", city);
            cmd.Parameters.AddWithValue("@json", json);
            cmd.Parameters.AddWithValue("@time", DateTime.Now);

            cmd.ExecuteNonQuery();
        }
    }
}
