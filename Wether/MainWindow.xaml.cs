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
            Init();
        }

        public async void Init()
        {
            response = await GetWether.Get(58.009671f, 56.226184f);
            Create(0);
            
        }
        public void Create(int idForecast)
        {
            foreach (Hour hour in response.forecasts[idForecast].hours)
            {
                parent.Children.Add(new Elements.Item(hour));
            }
        }
    }
}