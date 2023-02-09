using System;
using System.Collections;
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

namespace Sales
{
    /// <summary>
    /// Interaction logic for LinqWindow.xaml
    /// </summary>
    public partial class LinqWindow : Window
    {
        private LinqContext.DataContext context;
        public LinqWindow()
        {
            InitializeComponent();
            try
            {
                context = new(App.ConnectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Simple_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Products.OrderBy(p => p.Price);
            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Price + " " + item.Name + "\n";
            }
        }

        private void ByName_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Products.OrderBy(p => p.Name);
            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Price + " " + item.Name + "\n";
            }
        }

        private void ByPrice_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Products.OrderByDescending(p => p.Price);
            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Price + " " + item.Name + "\n";
            }
        }

        private void Less200_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Products
                        .Where(p => p.Price < 200)
                        .OrderBy(p => p.Price);
            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Price + " " + item.Name + "\n";
            }
        }

        private void withG_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Products
                            .Where(p => p.Name.StartsWith("Г"));
            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Price + " " + item.Name + "\n";
            }
            textBlock1.Text += "\n" + query.Count() + " Total";
        }

        private void withOV_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Products
                            .Where(p => p.Name.Contains("ов"));
            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Price + " " + item.Name + "\n";
            }
            textBlock1.Text += "\n" + query.Count() + " Total";
        }

        private void join_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Managers.Join(
                context.Departments,
                m => m.Id_main_dep,
                d => d.Id,
                (m, d) =>
                    new { Manager = m.Surname + " " + m.Name, Department = d.Name });

            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Manager + " - " + item.Department + "\n";
            }
            textBlock1.Text += "\n" + query.Count() + " Total";
        }

        private void mansdep_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Managers.Join(
                 context.Departments,
                 m => m.Id_sec_dep,
                 d => d.Id,
                 (m, d) =>
                     new { Manager = m.Surname + " " + m.Name, Department = d.Name });

            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Manager + " - " + item.Department + "\n";
            }
            textBlock1.Text += "\n" + query.Count() + " Total";
        }

        private void manchefp_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Managers.Join(
                context.Managers,
                m => m.Id_chief,
                d => d.Id,
                (m, d) =>
                    new { Manager = m.Surname + " " + m.Name, Chief = m.Surname + " " + m.Name });

            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Manager + " - " + item.Chief + "\n";
            }
            textBlock1.Text += "\n" + query.Count() + " Total";
        }

        private void full_Click(object sender, RoutedEventArgs e)
        {
            var query = context.Managers.Join(
                context.Managers,
                m => m.Id_chief, 
                d => d.Id, 
                (m, d) => m).Join(
                 context.Departments,
                 m => m.Id_sec_dep,
                 d => d.Id,
                 (m, d) =>
                     new { Manager = m.Surname + " " + m.Name, Department = d.Name, Chief = m.Surname + " " + m.Name });

            textBlock1.Text = "";
            foreach (var item in query)
            {
                textBlock1.Text += item.Manager + " - " + item.Department+ " - " + item.Chief + "\n";
            }
            textBlock1.Text += "\n" + query.Count() + " Total";
        }
    }
}