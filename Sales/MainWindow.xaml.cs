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
using System.Windows.Controls.Primitives;
using System.Threading;

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
            ShowTotalMonitorsCount();
            ShowTotalStatistics();
        }

        #region Show Monitors
        private void ShowTotalMonitorsCount()
        {
            SqlCommand cmd = new();
            cmd.Connection = _connection;

            cmd.CommandText = @$"SELECT COUNT(*) FROM Departments";
            MonitorDepartments.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = @$"SELECT COUNT(*) FROM Managers";
            MonitorManagers.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT COUNT(*) FROM Products";
            MonitorProducts.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT COUNT(*) FROM Sales";
            MonitorSales.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.Dispose();
        }
        #endregion

        #region Show Total Stats
        private void ShowTotalStatistics()
        {
            SqlCommand cmd = new();
            cmd.Connection = _connection;

            String date = $"2022-{DateTime.Now.Month}-{DateTime.Now.Day + 1}";

            cmd.CommandText = $@"SELECT COUNT(*) FROM Sales S WHERE CAST(S.Moment AS datetime) = '{date}'";
            StatTotalSales.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT COUNT(S.Cnt) FROM Sales S JOIN Products P ON P.Id = S.Id_product WHERE CAST(S.Moment AS datetime)  = '{date}'";
            StatTotalProducts.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT ROUND(SUM(S.Cnt * P.Price), 2) FROM Sales S JOIN Products P ON P.Id = S.Id_product WHERE CAST(S.Moment AS datetime)  = '{date}'";
            StatTotalMoney.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT TOP 1 CONCAT(m.Surname,' ', m.Name) FROM Sales s JOIN Managers m ON m.Id = s.Id_manager JOIN Products p ON p.Id = s.Id_product WHERE CAST(S.Moment AS datetime) = '{date}' GROUP BY m.Surname, m.Name ORDER BY SUM(p.Price)";
            StatTotalManager.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT TOP 1 d.Name FROM Departments d JOIN Managers m ON d.Id = m.Id_main_dep JOIN Sales s ON m.Id = s.Id_manager WHERE CAST(S.Moment AS datetime) = '{date}' GROUP BY d.Name ORDER BY COUNT(s.Cnt)";
            StatTotalDepartament.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.CommandText = $@"SELECT TOP 1 p.Name FROM Sales s JOIN Products p ON p.Id = s.Id_product WHERE CAST(S.Moment AS datetime) = '{date}' GROUP BY p.Name ORDER BY COUNT(s.Id_product)";
            StatTotalProduct.Content = Convert.ToString(cmd.ExecuteScalar());

            cmd.Dispose();
        }
        #endregion
    }
}