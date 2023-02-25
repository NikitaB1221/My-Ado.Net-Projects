using Sales.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sales
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // строка подключения - берется из свойств БД (Server Explorer)
        public const String ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\ШАГ\Ado.NET\ADO111\Sales\Sales111.mdf;Integrated Security=True";
        internal static readonly ILogger Logger = new DbLogger(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\ШАГ\Ado.NET\ADO111\Sales\logger_db.mdf;Integrated Security=True");
    }
}