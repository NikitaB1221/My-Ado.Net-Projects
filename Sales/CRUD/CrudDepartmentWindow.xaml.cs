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
using System.Windows.Shapes;

namespace Sales.CRUD
{
    /// <summary>
    /// Логика взаимодействия для CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {
        public Entities.Department Department { get; set; } = null!;

        public CrudDepartmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Department is null) // режим "С" - Добавление
            {
                Department = new Entities.Department()
                {
                    Id = Guid.NewGuid()
                };
                ButtonDelete.IsEnabled = false;
            }
            else // режимы "UD" - изменение/удаление
            {
                ButtonDelete.IsEnabled = true;
            }
            DepartmentId.Text = Department.Id.ToString();
            DepartmentName.Text = Department.Name;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (Department is null) return;

            Department.Name = DepartmentName.Text;
            if (Department.Name == String.Empty)
            {
                MessageBox.Show("Введите название отдела!");
                DepartmentName.Focus();
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Department = null;
            this.DialogResult = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
