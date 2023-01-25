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
using System.Windows.Markup;
using System.Data;

namespace Sales
{
    public partial class MainWindow : Window
    {
        private SqlConnection _connection;
        private List<Entities.Department>? _departments;
        private List<Entities.Product>? _products;
        private List<Entities.Manager>? _managers;
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
            ShowDepartmentsOrm();
            ShowProductsOrm();
            ShowManagersOrm();
            ShowSales();
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

            String date = $"2022-{DateTime.Now.Month}-{DateTime.Now.Day}";

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

        #region Show Departments

        private void ShowDepartments()
        {
            using SqlCommand cmd = new("SELECT id, name From Departments", _connection);
            SqlDataReader reader = cmd.ExecuteReader();
            DepartmentCell.Text = "";
            while (reader.Read())
            {
                Guid id = reader.GetGuid("id");
                String name = (String)reader[1];
                DepartmentCell.Text += $"{id} {name}\n";
            }
            reader.Dispose();
        }

        #endregion

        #region Show Departments Orm

        private void ShowDepartmentsOrm()
        {
            if (_departments is null)
            {
                using SqlCommand cmd = new("SELECT d.id, d.name From Departments d", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _departments = new();
                    while (reader.Read())
                    {
                        _departments.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            foreach (var department in _departments)
            {
                DepartmentCell.Text += department.ToShortString() + "\n";
            }
        }

        #endregion

        #region Show Products Orm

        private void ShowProductsOrm()
        {
            if (_products is null)
            {
                using SqlCommand cmd = new("SELECT p.id, p.name, p.price From Products p", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _products = new();
                    while (reader.Read())
                    {
                        _products.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2)
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            foreach (var department in _products)
            {
                ProductCell.Text += department.ToShortString() + "\n";
            }
        }

        #endregion

        #region Show Managers Orm

        private void ShowManagersOrm()
        {
            if (_managers is null)
            {
                using SqlCommand cmd = new("SELECT m.id, m.surname, m.name, m.secname, m.id_main_dep, m.id_sec_dep, id_chief From Managers m", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _managers = new();
                    while (reader.Read())
                    {
                        _managers.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Surname = reader.GetString(1),
                            Name = reader.GetString(2),
                            Secname = reader.GetString(3),
                            Id_main_dep = reader.GetGuid(4)
                            //Id_sec_dep = reader.GetGuid(5),
                            //Id_chief = reader.GetGuid(6)
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            foreach (var department in _managers)
            {
                ManagerCell.Text += department.ToShortString() + "\n";
            }
        }

        #endregion

        #region Show Sales

        private void ShowSales()
        {
            using SqlCommand cmd = new("SELECT p.id, p.Name, SUM(s.Cnt), SUM(p.Price) FROM Sales s JOIN Products p ON p.Id = s.Id_product GROUP BY p.Name, p.id", _connection);
            SqlDataReader reader = cmd.ExecuteReader();
            SaleCell.Text = "";
            while (reader.Read())
            {
                Guid Id = (Guid)reader[0];
                String name = (String)reader[1];
                double SumCnt = (int)reader[2];
                double SumPrice = (double)reader[3];
                SaleCell.Text += $"{Id.ToString()[..4]} {name}\t{SumCnt}\t{SumPrice}\n";
            }
            reader.Dispose();
        }
        #endregion
    }
}