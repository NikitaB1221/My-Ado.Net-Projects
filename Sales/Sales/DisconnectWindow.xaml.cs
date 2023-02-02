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
        public ObservableCollection<Entities.Department> Departments { get; set; } = null!;

        public ObservableCollection<Entities.Product> Products { get; set; } = null!;

        public ObservableCollection<Entities.Manager> Managers { get; set; } = null!;

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
                    //MessageBox.Show(department.ToShortString());
                    CRUD.CrudDepartmentWindow window = new()
                    {
                        Department = department
                    };
                    int index = Departments.IndexOf(department);
                    Departments.Remove(window.Department); // Костыль. Удаляем из колекции и передаем на редактирование
                    if (window.ShowDialog().GetValueOrDefault())
                    {
                        if (window.Department is null) // Удаление
                        {
                            using SqlConnection connection = new(App.ConnectionString);
                            try
                            {
                                connection.Open();
                                using SqlCommand cmd = new($"DELETE FROM Departments WHERE Id = @id", connection);
                                cmd.Parameters.AddWithValue("@id", department.Id);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                Close();
                            }
                        }
                        else // Изменение
                        {
                            Departments.Insert(index, department);
                            using SqlConnection connection = new(App.ConnectionString);
                            try
                            {
                                connection.Open();
                                using SqlCommand cmd = new($"UPDATE Departments SET Name = @name WHERE Id = @id", connection);
                                cmd.Parameters.AddWithValue("@id", department.Id);
                                cmd.Parameters.AddWithValue("@name", window.Department.Name);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                Close();
                            }
                        }
                    }
                    else // Отмена
                    {
                        Departments.Insert(index, department);
                    }
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

        private void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            var window = new CRUD.CrudDepartmentWindow();
            if (window.ShowDialog().GetValueOrDefault())
            {
                MessageBox.Show(window.Department.ToShortString());
                using SqlConnection connection = new(App.ConnectionString);
                try
                {
                    connection.Open();
                    using SqlCommand cmd = new("INSERT INTO Departments( Id, Name ) VALUES( @id, @name )", connection);
                    cmd.Parameters.AddWithValue("@id", window.Department.Id);
                    cmd.Parameters.AddWithValue("@name", window.Department.Name);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
                Departments.Add(window.Department);
            }
            else
            {
                MessageBox.Show("Cancel");
            }
        }
    }
}