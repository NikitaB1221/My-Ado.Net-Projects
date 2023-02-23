using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Sales.EFContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    /// Логика взаимодействия для EFWindow.xaml
    /// </summary>
    public partial class EFWindow : Window
    {
        public static Random rand = new Random();

        public EFContext.DataContext dataContext;

        public EFWindow()
        {
            InitializeComponent();
            dataContext = new();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MonitorDepartments.Content =
                dataContext.Departments.Count();
            MonitorProducts.Content =
                dataContext.Products.Count();
            MonitorManagers.Content =
                dataContext.Managers.Count();
            MonitorSales.Content = dataContext.Sales.Count();

            ShowDailyStatistics();
            NavProperties();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            int cntr = 1;
            int ManCount = rand.Next(0, dataContext.Managers.Count());
            int ProdCount = rand.Next(0, dataContext.Products.Count());
            while (cntr <= 10)
            {
                var obj = new Sales.EFContext.Sale()
                {


                    Id = Guid.NewGuid(),
                    Id_manager = dataContext.Managers.Skip(ManCount).First().Id,
                    Id_product = dataContext.Products.Skip(ProdCount).First().Id,
                    Cnt = rand.Next(1, 10),
                    Moment = new DateTime(
                        rand.Next(DateTime.Now.Year - 2, DateTime.Now.Year),
                        rand.Next(1, 12),
                        rand.Next(1, 30),
                        rand.Next(0, 23),
                        rand.Next(0, 59),
                        rand.Next(0, 59)
                        )
                };

                cntr++;
                dataContext.Sales.Add(obj);
            }
            dataContext.SaveChanges();
            MonitorSales.Content = dataContext.Sales.Count();
            ShowDailyStatistics();
        }

        private void ShowDailyStatistics()
        {
            SalesCnt.Content = dataContext.Sales.Where(sale => sale.Moment.Date == DateTime.Now.Date).Count();
            SalesTotal.Content = dataContext.Sales.Where(sale => sale.Moment.Date == DateTime.Now.Date).Sum(sale => sale.Cnt);
            MoneyTotal.Content = dataContext.Sales.Where(sale => sale.Moment.Date == DateTime.Now.Date).Join(dataContext.Products, sale => sale.Id_product, prod => prod.Id, (sale, prod) => sale.Cnt * prod.Price).Sum().ToString("0.000");

            SalesTopManager.Content = dataContext.Managers.GroupJoin(
                dataContext.Sales.Where(sale => sale.Moment.Date == DateTime.Now.Date),
                man => man.Id,
                sale => sale.Id_manager,
                (man, sales) => new
                {
                    Manager = man,
                    TotalSales = sales.Sum(s => s.Cnt),
                }).OrderByDescending(mix => mix.TotalSales).Take(1).Select(mix => mix.Manager.ToShortString() + $"({mix.TotalSales})").First();

            SalesTopProduct.Content = dataContext.Products.GroupJoin(
                dataContext.Sales.Where(sale => sale.Moment.Date == DateTime.Now.Date),
                prod => prod.Id,
                sale => sale.Id_product,
                (prod, sales) => new
                {
                    Product = prod,
                    TotalSales = sales.Sum(s => s.Cnt),
                    TotalMoney = prod.Price * sales.Sum(s => s.Cnt)
                }).OrderByDescending(mix => mix.TotalSales).Take(1).Select(mix => mix.Product.ToShortString() + $"({mix.TotalSales}) - {mix.TotalMoney.ToString("0.000")}$").First();

            SalesTopDepartment.Content = dataContext.Departments
            .Join(dataContext.Managers, d => d.Id, m => m.Id_main_dep, (d, m) => new { Dep = d, Man = m })
                .GroupJoin(
                    dataContext.Sales.Where(s => s.Moment.Date == DateTime.Now.Date),
                    dm => dm.Man.Id,
                    sale => sale.Id_manager,
                    (dm, sales) => new { Dep = dm.Dep, Man = dm.Man, Total = sales.Sum(sale => sale.Cnt) }
                ).ToList()
            .GroupBy(
                dms => dms.Dep,
                dms => dms.Total,
                (dep, ts) => new { Dep = dep, Total = ts.Sum() }
            )
            .OrderByDescending(dt => dt.Total)
            .Select(dt => dt.Dep.Name + $"({dt.Total})")
            .First();
        }

        private void NavProperties()
        {
            //var man = dataContext.Managers.Include(m => m.MainDep).Include(m => m.Chief).First();
            //label1.Content = man.Surname + " " + man.Name[0] + ". " + man.MainDep.Name;
            //label1.Content += '\n' + (man.Chief?.Surname ?? "--") + " " + (man.Chief?.Name ?? "--") + " " + man.Subordinates.Count() + "подч-ныйе";

            //var dep = dataContext.Departments.Include( d => d.Managers).Include(d => d.PartWorker).Skip(1).First();
            //label1.Content += '\n' + dep.Name + ": " + dep.Managers.Count() + " сотр., " + dep.PartWorker.Count() + " совм.сотр.";


        }
    }
}