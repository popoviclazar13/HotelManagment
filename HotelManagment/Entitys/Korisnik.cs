using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Korisnik
    {
        [Key]
        public int korisnikId { get; set; }

        [Required]
        public string imePrezime { get; set; }

        public string telefon { get; set; }

        public string email { get; set; }
        public string zemlja { get; set; }

        // Lista rezervacija koje je korisnik napravio
        public List<Rezervacija> listaRezervacija { get; set; } = new List<Rezervacija>();
    }
}
