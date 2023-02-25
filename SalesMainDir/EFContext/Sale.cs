using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.EFContext
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid Id_manager { get; set; }
        public Guid Id_product { get; set; }
        public int Cnt { get; set; }
        public DateTime Moment { get; set; }
    }
}
