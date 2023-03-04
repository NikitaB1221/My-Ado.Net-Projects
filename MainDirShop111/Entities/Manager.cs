using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop111.Entities
{
    public class Manager
    {
        public Guid Id { get; set; }
        public String Surname { get; set; } = null!;
        public String Name { get; set; } = null!;
        public String Secname { get; set; } = null!;
        public Guid Id_main_dep { get; set; }
        public Guid? Id_sec_dep { get; set; }
        public Guid? Id_chief { get; set; }

        public Department MainDep { get; set; } = null!;
        public Department? SecDep { get; set; }

        public Manager? Chief { get; set; }
        public List<Manager> Subordinates { get; set; }

        public List<Sale> Sales { get; set; }   

        public String ToShortString()
        {
            return $"{Surname} {Name[0]}. {Secname[0]}.";
        }
    }
}

