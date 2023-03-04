using Microsoft.EntityFrameworkCore;
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

namespace Shop111
{
    /// <summary>
    /// Логика взаимодействия для PersonelWindow.xaml
    /// </summary>
    public partial class PersonelWindow : Window
    {
        public ObservableCollection<Entities.Manager> Managers { get; set; }

        public Entities.DataContext dataContext;

        public PersonelWindow()
        {
            InitializeComponent();

            DataContext = this;
            using SqlConnection connection = new(App.ConnectionString);

            dataContext = new();

            try
            {
                connection.Open();

                Managers = new();
                using SqlCommand cmd2 = new("SELECT Id, Surname, Name, Secname FROM Managers", connection);
                {
                    using var reader = cmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        Managers.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Surname = reader.GetString(1),
                            Name = reader.GetString(2),
                            Secname = reader.GetString(3),
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PersonelLV.SelectedItem as Manager != null)
            {
                var tmpMan = PersonelLV.SelectedItem as Manager;
                var man = dataContext.Managers.Include(m => m.Sales).Include(m => m.MainDep).Where(s => s.Id == tmpMan!.Id).First();
                SalesListLabel.Content = "Full name: " + man.Surname + " " + man.Name + " " + man.Secname + "\n"
                    + "Department: " + man.MainDep.Name + "\n"
                    + "Id: " + man.Id + "\n"
                    + "Number of Sales: " + man.Sales.Count() + "\n\n";
            }
            else return;
        }
    }
}
