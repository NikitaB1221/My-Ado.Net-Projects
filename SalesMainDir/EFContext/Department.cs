using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.EFContext
{
    public class Department
    {
        public Guid Id { get; set; }                 // Набор свойств сущности в точности
        public String Name { get; set; } = null!;    // повторяет структуру таблицы
        
        public List<Manager> Managers { get; set; }

        public List<Manager> PartWorker { get; set; }

        public String ToShortString()
        {
            return Id.ToString()[..4] + "... " + Name;
        }
    }
}
