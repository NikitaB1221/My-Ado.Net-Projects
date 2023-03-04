using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shop111.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Shop111
{
    /// <summary>
    /// Логика взаимодействия для SalesHistoryWindow.xaml
    /// </summary>
    public partial class SalesHistoryWindow : Window
    {
        public ObservableCollection<Entities.Sale> Sales { get; set; }

        public Entities.DataContext dataContext;


        public SalesHistoryWindow()
        {
            InitializeComponent();
            DataContext = this;
            using SqlConnection connection = new(App.ConnectionString);

            dataContext = new();

            try
            {
                connection.Open();

                #region Sales
                Sales = new();
                using SqlCommand cmd2 = new("SELECT Id, Id_manager, Id_product, Cnt, Moment FROM Sales", connection);
                {
                    using var reader = cmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        Sales.Add(new()  // Изменение коллекции отобразиться на ListView
                        {
                            Id = reader.GetGuid(0),
                            Id_manager = reader.GetGuid(1),
                            Id_product = reader.GetGuid(2),
                            Cnt = reader.GetInt32(3),
                            Moment = reader.GetDateTime(4)
                        });
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void ListViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SalesHistiryLV.SelectedItem as Sale != null)
            {
                var tmpSale = SalesHistiryLV.SelectedItem as Sale;
                var sale = dataContext.Sales.Include(s => s.Manager).Include(s => s.Product).Where(s => s.Id == tmpSale!.Id).First();
                SalesListLabel.Content = "Product: " + sale.Product.Name + " " + sale.Cnt + " x - " + (sale.Product.Price * sale.Cnt) + " $\n"
                    + "Sold by: " + sale.Manager.Surname + " " + sale.Manager.Name[0] + ". " + sale.Manager.Secname[0] + ".\n"
                    + "Time: " + sale.Moment;
            }
            else return;
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (SalesHistiryLV.SelectedItem as Sale != null)
            {
                var MB = MessageBox.Show("Are you sure?", "Delete Entry", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (MB == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (SalesHistiryLV.SelectedItem as Sale != null)
                        {

                            var tmpSale = SalesHistiryLV.SelectedItem as Sale;
                            //dataContext.Sales.Include(s => s.Manager).Include(s => s.Product).Where(s => s.Id == tmpSale!.Id).ExecuteDelete();
                            var sale = (from h in dataContext.Sales where h.Id == tmpSale!.Id select h).SingleOrDefault();
                            dataContext.Sales.Remove(sale!);
                            dataContext.SaveChanges();

                            Sales.Remove((Entities.Sale)SalesHistiryLV.SelectedItem);
                        }
                        else return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Close();
                    }
                }
            }
            else return;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SalesListLabel.Content = "Product: N/A N/A x - N/A $ \nSold by: N/A n/a. n/a. \nTime: N/A";
        }

        private void CleanTheHistory_Click(object sender, RoutedEventArgs e)
        {
            var MB = MessageBox.Show("Are you sure?", "Delete Entry", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (MB == MessageBoxResult.Yes)
            {
                try
                {

                    dataContext.Sales.RemoveRange(dataContext.Sales);
                    dataContext.SaveChanges();
                    Sales.Clear();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
            else return;
        }
    }
}
