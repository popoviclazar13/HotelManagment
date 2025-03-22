using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Rezervacija
    {
        [Key]
        public int rezervacijaId { get; set; }
        public DateTime pocetniDatum { get; set; }
        public DateTime krajnjiDatum { get; set; }
        public int brojGostiju { get; set; }
        public double ukupnaCena { get; set; }
        public double cenaKonacna { get; set; }
        public string komentar { get; set; }
        public Boolean placeno { get; set; }
        public string nacinPlacanja { get; set; }
        public double iznosProvizije { get; set; }
        //Ovo ne treba da bude u bazi
        public string PlacenoTekst => placeno ? "DA" : "NE";
        //
        [ForeignKey("apartman")]
        public int apartmanId { get; set; }
        public Apartman apartman { get; set; } // Navigacija ka apartmanu

        [ForeignKey("korisnik")]
        public int korisnikId { get; set; }
        public Korisnik korisnik { get; set; } // Navigacija ka korisniku

        [ForeignKey("agencija")]
        public int? agencijaId { get; set; }
        public Agencija agencija { get; set; } // Navigacija ka agenciji (može biti null)

        public List<RezervacijaUsluga> listaRezervacijaUsluga { get; set; } = new List<RezervacijaUsluga>();
    }
}
