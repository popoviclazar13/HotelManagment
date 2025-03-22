using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class ApartmanPopust
    {
        public int apartmanPopustId { get; set; }
        [ForeignKey("apartman")]
        public int apartmanId { get; set; }
        public Apartman apartman { get; set; } // Navigacija ka apartmanu

        [ForeignKey("popust")]
        public int popustId { get; set; }
        public Popust popust { get; set; } // Navigacija ka popust
    }
}
