using Newtonsoft.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wether.Classes;
using Wether.Model;

namespace Wether
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataResponse response;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void FindCity(object sender, RoutedEventArgs e)
        {
            string city = CityBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(city))
            {
                MessageBox.Show("Введите название города.");
                return;
            }

            await LoadWeather(city);
        }
        private async Task LoadWeather(string city)
        {
            string json;
            if (CacheService.TryGetWeather(city, out json))
            {
                response = JsonConvert.DeserializeObject<DataResponse>(json);
                UpdateUI();
                return;
            }

            try
            {
                var (lat, lon) = await GetWether.GeoCoder.GetCoords(city);
                json = await GetWether.GetJson(lat, lon);
                CacheService.SaveWeather(city, json);
                response = JsonConvert.DeserializeObject<DataResponse>(json);

                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void UpdateUI()
        {
            Days.Items.Clear();

            foreach (Forecast forecast in response.forecasts)
                Days.Items.Add(forecast.date.ToString("dd.MM.yyyy"));

            Days.SelectedIndex = 0;
            Create(0);
        }

        public void Create(int idForecast)
        {
            parent.Children.Clear();

            foreach (Hour h in response.forecasts[idForecast].hours)
                parent.Children.Add(new Elements.Item(h));
        }

        private void SelectDays(object sender, RoutedEventArgs e)
        {
            if (Days.SelectedIndex >= 0)
                Create(Days.SelectedIndex);
        }

        private async void UpdateWeather(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CityBox.Text))
                await LoadWeather(CityBox.Text);
        }


    }
}