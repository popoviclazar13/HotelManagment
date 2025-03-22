using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class RezervacijaUsluga
    {
        [Key]
        public int rezervacijaUslugaId { get; set; }
        [Required]
        public int kolicina { get; set; }
        [Required]
        public DateTime datum { get; set; }

        [ForeignKey("rezervacija")]
        public int rezervacijaId { get; set; }
        public Rezervacija rezervacija { get; set; } // Navigacija ka rezervacija

        [ForeignKey("usluga")]
        public int uslugaId { get; set; }
        public Usluga usluga { get; set; } // Navigacija ka korisniku
    }
}
