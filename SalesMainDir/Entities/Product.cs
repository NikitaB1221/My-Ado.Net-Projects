using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;
        public double Price { get; set; }

        public String ToShortString()
        {
            return Id.ToString()[..4] + " " + Name + " " + Price;
        }
    }
}
