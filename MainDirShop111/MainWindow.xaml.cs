using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shop111
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ImageBrush imBrush = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri("D:/ШАГ/Ado.NET/Shop111/Shop111/FSImage.jpg"))
        };

        public MainWindow()
        {
            InitializeComponent();
            this.Background = imBrush;
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var CW = new CatalogWindow();
            CW.Owner = this;
            CW.ShowDialog();
            this.Show();
        }

        private void CheckHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var SHW = new SalesHistoryWindow();
            SHW.ShowDialog();
            this.Show();
        }

        private void PersList_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new PersonelWindow().ShowDialog();
            this.Show();
        }
    }
}
