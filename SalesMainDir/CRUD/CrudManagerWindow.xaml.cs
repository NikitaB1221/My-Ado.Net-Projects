using Sales.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CrudManagerWindow.xaml
    /// </summary>
    public partial class CrudManagerWindow : Window
    {
        public Entities.Manager Manager { get; set; } = null!;

        public CrudManagerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = (Owner as DisconnectWindow);
            if (Manager is null)  // режим "C" - добавление отдела
            {
                Manager = new Entities.Manager()
                {
                    Id = Guid.NewGuid()
                };
                ButtonDelete.IsEnabled = false;
            }
            else  // Режимы "UD" - есть переданный отдел для изменения/удаления
            {
                ButtonDelete.IsEnabled = true;
            }
            ManagerId.Text = Manager.Id.ToString();
            ManagerSurname.Text = Manager.Surname;
            ManagerName.Text = Manager.Name;
            ManagerSecname.Text = Manager.Secname;
            var query = from d in (Owner as DisconnectWindow).Departments where d.Id == Manager.Id_main_dep select d.Name;
            foreach (var item in query)
            {
                DepartmentsCombo.Text = item;
            }
            query = from d in (Owner as DisconnectWindow).Departments where d.Id == Manager.Id_sec_dep select d.Name;
            foreach (var item in query)
            {
                SecondaryCombo.Text = item;
            }
            query = from d in (Owner as DisconnectWindow).Managers where d.Id == Manager.Id_chief select d.Name;
            foreach (var item in query)
            {
                ChiefCombo.Text = item;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DataContext = (Owner as DisconnectWindow);
            if (Manager is null) return;

            if (ManagerName.Text == String.Empty)
            {
                MessageBox.Show("Введите имя сотрудника!");
                ManagerName.Focus();
            }
            else if (ManagerSurname.Text == String.Empty)
            {
                MessageBox.Show("Введите фамилию сотрудника!");
                ManagerName.Focus();
            }
            else if (ManagerSecname.Text == String.Empty)
            {
                MessageBox.Show("Введите отчество сотрудника!");
                ManagerName.Focus();
            }
            else if (ManagerSecname.Text == String.Empty)
            {
                MessageBox.Show("Введите отчество сотрудника!");
                ManagerName.Focus();
            }
            else if (DepartmentsCombo.Text == String.Empty)
            {
                MessageBox.Show("Выберите отдел сотрудника!");
                ManagerName.Focus();
            }
            else
            {
                Manager.Name = ManagerName.Text;
                Manager.Surname = ManagerSurname.Text;
                Manager.Secname = ManagerSecname.Text;
                var query = from d in (Owner as DisconnectWindow).Departments where d.Name == DepartmentsCombo.Text select d.Id;
                foreach (var item in query)
                {
                    Manager.Id_main_dep = item;
                }
                query = from d in (Owner as DisconnectWindow).Departments where d.Name == SecondaryCombo.Text select d.Id;
                foreach (var item in query)
                {
                    Manager.Id_sec_dep = item;
                }
                query = from d in (Owner as DisconnectWindow).Managers where d.Name == ChiefCombo.Text select d.Id;
                foreach (var item in query)
                {
                    Manager.Id_chief = item;
                }
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
        MessageBox.Show(
            "Вы подтверждаете удаление сотрудника из БД?",
            "Удаление данных из БД",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question))
            {
                Manager = null!;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}