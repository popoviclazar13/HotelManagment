using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class TipApartmana
    {
        [Key]
        public int tipApartmanaId { get; set; }

        [Required]
        public string nazivTipaApartmana { get; set; }


        // Navigaciono svojstvo ka apartmanima ovog tipa
        public List<Apartman> listaApartmana { get; set; } = new List<Apartman>();
    }
}
