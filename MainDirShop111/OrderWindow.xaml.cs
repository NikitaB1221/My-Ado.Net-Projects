using Shop111.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shop111
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public static Random rand = new Random();

        public Entities.DataContext dataContext = new();


        internal int CntV = 1;
        internal double PriceV;

        public OrderWindow()
        {
            InitializeComponent();

            DataContext = (Owner as CatalogWindow);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }



        private void timer_Tick(object? sender, EventArgs e)
        {
            TimeLabel.Content = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
            
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

            int ManCount = rand.Next(0, dataContext.Managers.Count());

            try
            {
                var obj = new Shop111.Entities.Sale()
                {
                    Id = Guid.NewGuid(),
                    Id_manager = dataContext.Managers.Skip(ManCount).First().Id,
                    Id_product = (Owner as CatalogWindow)!.prodId,
                    Cnt = CntV,
                    Moment = DateTime.Now
                };

                dataContext.Sales.Add(obj);
                dataContext.SaveChanges();

                MessageBox.Show($"Id - {obj.Id}\n Manager's Id - {obj.Id_manager}\n Product: {ProductLabel.Content} - {PriceLabel.Content}\n\n\t{obj.Moment.Hour}:{obj.Moment.Minute}:{obj.Moment.Second}", Title = "\tCheck", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            CntV = 1;
            
            this.Close();
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProductLabel.Content = (Owner as CatalogWindow)?.TitleLabel.Content;
            PriceLabel.Content = (Owner as CatalogWindow)?.PriceLabel.Content;
            PriceV = Double.Parse(PriceLabel.Content?.ToString());
            CntTB.Text = $"{CntV}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CntV < 10)
            {
                CntV++;
            }
            CntTB.Text = CntV.ToString();
            PriceLabel.Content = $"{PriceV * CntV}";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CntV > 1)
            {
                CntV--;
            }

            CntTB.Text = CntV.ToString();
            PriceLabel.Content = $"{PriceV * CntV}";
        }
    }
}
