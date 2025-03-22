using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class CenaApartmana
    {
        [Key]
        public int cenaApartmanaId { get; set; }
        public DateTime pocetniDatum { get; set; }
        public DateTime krajnjiDatum { get; set; }
        public double cenaPoOsobi { get; set; }
        public double cenaPoNoci { get; set; }
        [ForeignKey("Apartman")]
        public int apartmanId { get; set; }
        public Apartman apartman { get; set; } // Navigacija ka apartmanu
    }
}
