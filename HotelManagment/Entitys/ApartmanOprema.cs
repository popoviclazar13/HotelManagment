using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class ApartmanOprema
    {
        [Key]
        public int apartmanOpremaId { get; set; }
        [Required]
        public int kolicinaOpreme { get; set; }
        [ForeignKey("apartman")]
        public int apartmanId { get; set; }
        public Apartman apartman { get; set; } // Navigacija ka apartmanu

        [ForeignKey("oprema")]
        public int opremaId { get; set; }
        public Oprema oprema { get; set; } // Navigacija ka oprema
    }
}
