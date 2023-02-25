using Sales.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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
            using SqlConnection connection = new(App.ConnectionString);
            try
            {
                connection.Open();

                #region Departments
                Departments = new();
                using SqlCommand cmd = new("SELECT Id, Name FROM Departments", connection);
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Departments.Add(new()  // Изменение коллекции отобразиться на ListView
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                #endregion

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

                #region Managers
                Managers = new();
                using SqlCommand cmd3 = new("SELECT Id, Surname, Name, Secname, Id_main_dep, Id_sec_dep, Id_chief FROM Managers", connection);
                {
                    using var reader = cmd3.ExecuteReader();
                    while (reader.Read())
                    {
                        Managers.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Surname = reader.GetString(1),
                            Name = reader.GetString(2),
                            Secname = reader.GetString(3),
                            Id_main_dep = reader.GetGuid(4),
                            Id_sec_dep = reader[5] == DBNull.Value   // В БД любой элемент может быть NULL
                                        ? null                     // но в С# значимые типы не могут
                                        : reader.GetGuid(5),       // принимать null значение
                            Id_chief = reader[6] == DBNull.Value    // Для передачи значимых типов, но опциональных
                                        ? null                     // сначала запрашивают object, его проверяют
                                        : reader.GetGuid(6)        // на DBNull.Value и если это не так, то
                        });                                        // повторяют запрос со значимым Get-тером (GetGuid)
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

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)  // item = sender as ListViewItem
            {
                // Обратная связь (view->model) через item.Content
                if (item.Content is Entities.Department department)
                {
                    // MessageBox.Show(department.ToShortString());
                    CRUD.CrudDepartmentWindow window = new()
                    {
                        Department = department
                    };
                    int index = Departments.IndexOf(department);
                    Departments.Remove(department);  // удаляем из коллекции и передаем на редактирование
                    if (window.ShowDialog().GetValueOrDefault())
                    {
                        using SqlConnection connection = new(App.ConnectionString);
                        try
                        {
                            connection.Open();
                            using SqlCommand cmd = new() { Connection = connection };
                            if (window.Department is null)  // удаление
                            {
                                cmd.CommandText = $"DELETE FROM Departments WHERE Id = '{department.Id}' ";
                            }
                            else  // изменение
                            {
                                cmd.CommandText = $"UPDATE Departments SET Name = @name WHERE Id = '{department.Id}' ";
                                cmd.Parameters.AddWithValue("@name", department.Name);
                                Departments.Insert(index, department);  // возвращаем, но измененный
                            }
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Задание выполнено успешно");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else  // Отмена - возвращаем в окно
                    {
                        Departments.Insert(index, department);
                    }
                }
            }

        }

        private void ListViewItem_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)  // item = sender as ListViewItem
            {
                // Обратная связь (view->model) через item.Content
                if (item.Content is Entities.Product product)
                {
                    MessageBox.Show(product.ToShortString());
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
                    using SqlCommand cmd = new(
                        $"INSERT INTO Departments(Id, Name) VALUES( @id, @name )",
                        connection);
                    cmd.Parameters.AddWithValue("@id", window.Department.Id);
                    cmd.Parameters.AddWithValue("@name", window.Department.Name);
                    cmd.ExecuteNonQuery();

                    Departments.Add(window.Department);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Cancel");
            }
        }

        private void AddManager_Click(object sender, RoutedEventArgs e)
        {
            var window = new CRUD.CrudManagerWindow() { Owner = this };

            if (window.ShowDialog().GetValueOrDefault())
            {
                MessageBox.Show(window.Manager.ToShortString());
                using SqlConnection connection = new(App.ConnectionString);
                try
                {
                    connection.Open();
                    using SqlCommand cmd = new(
                        $"INSERT INTO Managers(Id, Surname, Name, Secname, Id_main_dep, Id_sec_dep, Id_chief) VALUES( @id, @surname, @name, @secname, @id_main_dep, @id_sec_dep, @id_chief)",
                        connection);
                    cmd.Parameters.AddWithValue("@id", window.Manager.Id);
                    cmd.Parameters.AddWithValue("@surname", window.Manager.Surname);
                    cmd.Parameters.AddWithValue("@name", window.Manager.Name);
                    cmd.Parameters.AddWithValue("@secname", window.Manager.Secname);
                    cmd.Parameters.AddWithValue("@id_main_dep", window.Manager.Id_main_dep);
                    cmd.Parameters.AddWithValue("@id_sec_dep", window.Manager.Id_sec_dep);
                    cmd.Parameters.AddWithValue("@id_chief", window.Manager.Id_chief);
                    cmd.ExecuteNonQuery();

                    Managers.Add(window.Manager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Cancel");
            }
        }

        private void ListViewItem_MouseDoubleClick_2(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)  // item = sender as ListViewItem
            {
                // Обратная связь (view->model) через item.Content
                if (item.Content is Entities.Manager manager)
                {
                    // MessageBox.Show(department.ToShortString());
                    CRUD.CrudManagerWindow window = new()
                    {
                        Owner = this,
                        Manager = manager,
                    };
                    int index = Managers.IndexOf(manager);
                    Managers.Remove(manager);  // удаляем из коллекции и передаем на редактирование
                    if (window.ShowDialog().GetValueOrDefault())
                    {
                        using SqlConnection connection = new(App.ConnectionString);
                        try
                        {
                            connection.Open();
                            using SqlCommand cmd = new() { Connection = connection };
                            if (window.Manager is null)  // удаление
                            {
                                cmd.CommandText = $"DELETE FROM Managers WHERE Id = '{manager.Id}' ";
                            }
                            else  // изменение
                            {
                                cmd.CommandText = $"UPDATE Managers SET Name = @name, Surname = @surname, Secrname = @secname, Id_main_dep = @id_main_dep, Id_sec_dep = @id_sec_dep, Id_chief = @id_chief WHERE Id = '{manager.Id}' ";
                                cmd.Parameters.AddWithValue("@surname", manager.Surname);
                                cmd.Parameters.AddWithValue("@name", manager.Name);
                                cmd.Parameters.AddWithValue("@secname", manager.Secname);
                                cmd.Parameters.AddWithValue("@id_main_dep", manager.Id_main_dep);
                                cmd.Parameters.AddWithValue("@id_sec_dep", manager.Id_sec_dep);
                                cmd.Parameters.AddWithValue("@id_chief", manager.Id_chief);
                                Managers.Insert(index, manager);  // возвращаем, но измененный
                            }
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Задание выполнено успешно");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else  // Отмена - возвращаем в окно
                    {
                        Managers.Insert(index, manager);
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Guid id = Guid.Parse("D3C376E4-BCE3-4D85-ABA4-E3CF49612C94");
            //LINQ query form
            var query = from d in Departments where d.Id == id select d.Name;

            //LINQ extension / method form
            var extmethod = Departments.FirstOrDefault(d => d.Id == id);

            foreach (String name in query)
            {
                textBlock1.Text += "\t" + name + "\n";
            }
            textBlock1.Text += "--------------------------------------\n";
            textBlock1.Text += "\t" + extmethod!.Name + "\n";


            var man = Managers[3];

            var maindep = Departments.FirstOrDefault(d => d.Id == man.Id_main_dep);
            var secdep = Departments.FirstOrDefault(d => d.Id == man.Id_sec_dep);
            var chief = Departments.FirstOrDefault(d => d.Id == man.Id_chief);

            textBlock1.Text += man.Name + "  " + maindep!.Name + "  " + (secdep?.Name ?? "--") + "  " + (chief?.Name ?? "--");
        }
    }
}