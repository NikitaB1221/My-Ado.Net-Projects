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

using System.Data.SqlClient;

namespace Sales
{
    public partial class MainWindow : Window
    {
        private SqlConnection _connection;
        public MainWindow()
        {
            InitializeComponent();
            String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\ШАГ\Ado.NET\ADO111\Sales\Sales111.mdf;Integrated Security=True";
            _connection = new(connectionString);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                MonitorConnection.Content = "Установлено";
                MonitorConnection.Foreground = Brushes.Green;
            }
            catch (SqlException ex)
            {
                MonitorConnection.Content = "Закрыто";
                MonitorConnection.Foreground = Brushes.Red;
                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowDepartmentsCount();
            ShowManagersCount();
            ShowProductsCount();
        }

        private void ShowDepartmentsCount()
        {
            String sql = "SELECT COUNT(*) FROM Departments";
            using var cmd = new SqlCommand(sql, _connection);
            MonitorDepartments.Content =
                Convert.ToString(
                    cmd.ExecuteScalar()
                );
        }

        private void ShowManagersCount()
        {
            String sql = "SELECT COUNT(*) FROM Managers";
            using var cmd = new SqlCommand(sql, _connection);
            MonitorManagers.Content =
                Convert.ToString(
                    cmd.ExecuteScalar()
                );
        }

        private void ShowProductsCount()
        {
            String sql = "SELECT COUNT(*) FROM Products";
            using var cmd = new SqlCommand(sql, _connection);
            MonitorProducts.Content =
                Convert.ToString(
                    cmd.ExecuteScalar()
                );
        }
    }
}