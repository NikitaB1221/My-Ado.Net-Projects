using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Shop111
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const String ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=EfSales111;Integrated Security=True";
    }
}
