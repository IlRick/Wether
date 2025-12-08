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

        // ------------------------- ЗАГРУЗКА ПОГОДЫ -------------------------
        private async Task LoadWeather(string city)
        {
            string json;

            // 1) Проверяем кэш
            if (CacheService.TryGetWeather(city, out json))
            {
                response = JsonConvert.DeserializeObject<DataResponse>(json);
                UpdateUI();
                return;
            }

            try
            {
                // 2) Получаем координаты
                var (lat, lon) = await GetWether.GeoCoder.GetCoords(city);

                // 3) Запрашиваем данные у API
                json = await GetWether.GetJson(lat, lon);

                // 4) Сохраняем в БД
                CacheService.SaveWeather(city, json);

                // 5) Загружаем в приложение
                response = JsonConvert.DeserializeObject<DataResponse>(json);

                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // --------------------- ОБНОВЛЕНИЕ ИНТЕРФЕЙСА ---------------------
        private void UpdateUI()
        {
            Days.Items.Clear();

            foreach (Forecast forecast in response.forecasts)
                Days.Items.Add(forecast.date.ToString("dd.MM.yyyy"));

            Days.SelectedIndex = 0;
            Create(0);
        }

        // ------------------------ ЗАПОЛНЕНИЕ ЧАСОВ ------------------------
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