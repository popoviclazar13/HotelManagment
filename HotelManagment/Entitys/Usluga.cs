using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Usluga
    {
        [Key]
        public int uslugaId { get; set; }
        [Required]
        public string nazivUsluge { get; set; }
        [Required]
        public double cenaUsluge { get; set; }

        public List<RezervacijaUsluga> listaRezervacijaUsluga { get; set; } = new List<RezervacijaUsluga>();
    }
}
