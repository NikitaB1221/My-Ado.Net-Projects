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

namespace Sales
{
    /// <summary>
    /// Interaction logic for DisconnectWindow.xaml
    /// </summary>
    public partial class DisconnectWindow : Window
    {
        public ObservableCollection<Entities.Department> Departments { get; set; }

        public ObservableCollection<Entities.Product> Products { get; set; }

        public ObservableCollection<Entities.Manager> Managers { get; set; }

        public DisconnectWindow()
        {
            InitializeComponent();
            // Связывание. Часть 1. Контекст
            DataContext = this;  // Представление получает доступ к всему объекту окна
            DepartmentsAdd();
            ProductsAdd();
            ManagersAdd();
        }

        private void DepartmentsAdd()
        {
            using SqlConnection connection = new(App.ConnectionString);
            try
            {
                connection.Open();
                Departments = new();
                using SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM Departments", connection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Departments.Add(new()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void ProductsAdd()
        {
            using SqlConnection connection = new(App.ConnectionString);
            try
            {
                connection.Open();
                Products = new();
                using SqlCommand cmd = new SqlCommand("SELECT Id, Name, Price FROM Products", connection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new()
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDouble(2)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void ManagersAdd()
        {
            using SqlConnection connection = new(App.ConnectionString);
            try
            {
                connection.Open();
                Managers = new();
                using SqlCommand cmd = new SqlCommand("SELECT Id, Surname, Name, Secname FROM Managers", connection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(new()
                    {
                        Id = reader.GetGuid(0),
                        Surname = reader.GetString(1),
                        Name = reader.GetString(2),
                        Secname = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void ListViewItem_MouseDoubleClick_Department(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Department department)
                {
                    MessageBox.Show(department.ToShortString());
                }
            }
        }

        private void ListViewItem_MouseDoubleClick_Product(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Product product)
                {
                    MessageBox.Show(product.ToShortString());
                }
            }
        }

        private void ListViewItem_MouseDoubleClick_Managers(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Manager manager)
                {
                    MessageBox.Show(manager.ToShortString());
                }
            }
        }
    }
}