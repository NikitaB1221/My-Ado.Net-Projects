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

namespace Shop111
{
    /// <summary>
    /// Логика взаимодействия для CatalogWindow.xaml
    /// </summary>
    public partial class CatalogWindow : Window
    {
        public ObservableCollection<Entities.Product> Products { get; set; }

        public Entities.DataContext dataContext = new();

        public CatalogWindow()
        {
            InitializeComponent();

            DataContext = this;
            using SqlConnection connection = new(App.ConnectionString);

            try
            {
                connection.Open();

                #region Products
                Products = new();
                using SqlCommand cmd2 = new("SELECT Id, Name, Price FROM Products", connection);
                {
                    using var reader = cmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        Products.Add(new()  // Изменение коллекции отобразиться на ListView
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2)
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

        public Guid prodId;

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Product product)
                {
                    CreateButton.IsEnabled = true;
                    TitleLabel.Content = product.Name;
                    PriceLabel.Content = product.Price;
                    prodId = product.Id;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var OW = new OrderWindow();
            OW.Owner = this;
            OW.ShowDialog();
            CreateButton.IsEnabled = false;
            this.ShowDialog();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateButton.IsEnabled = false;
        }
    }
}
