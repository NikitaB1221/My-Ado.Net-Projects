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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int cntr = 1;
            while (cntr <= 10 )
            {
                var obj = new Sales.EFContext.Sale()
                {
                    Id = Guid.NewGuid(),
                    Id_manager = dataContext.Managers.Skip(new Random().Next(1, dataContext.Managers.Count())).First().Id,
                    Id_product = dataContext.Products.Skip(new Random().Next(1, dataContext.Products.Count())).First().Id,
                    Cnt = new Random().Next(1, 10),
                    Moment = new DateTime( 
                        new Random().Next(DateTime.Now.Year - 2, DateTime.Now.Year),
                        new Random().Next(1, 12),
                        new Random().Next(1, 30),
                        new Random().Next(0, 23),
                        new Random().Next(0, 59),
                        new Random().Next(0, 59)
                        )
                };

                cntr++;
                dataContext.Sales.Add(obj);
            }
            dataContext.SaveChanges();
            MonitorSales.Content = dataContext.Sales.Count();
        }
    }
}


/*
* DECLARE @I INT
* SET     @I = 0
* SET     NOCOUNT ON
* WHILE   @I < 100000
* BEGIN
*   SET @I = @I + 1 
*   INSERT INTO Sales 
*     ( Id_manager, Id_product, Moment, Cnt)
*   VALUES
*   (
*     ( SELECT TOP 1 Id FROM Managers ORDER BY NEWID() ),        -- random ID from Manager
*     ( SELECT TOP 1 Id FROM Products ORDER BY NEWID() ),        -- random ID from Products
*     ('2022-01-01'                          -- base date - first day in year
*       + DATEADD( day,    ( ABS( CHECKSUM( NEWID() ) ) % 365 ), 0) -- random day - one from 365
*       + DATEADD( hour,   ( ABS( CHECKSUM( NEWID() ) ) % 24  ), 0) -- random hour - one from 24
*       + DATEADD( minute, ( ABS( CHECKSUM( NEWID() ) ) % 60  ), 0) -- random minute - one from 60
*     ),
*     ( ABS( CHECKSUM( NEWID() ) ) % 10 + 1 )              -- random Cnt: 1 to 10
*   )
* END
*/