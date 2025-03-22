using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Apartman
    {
        [Key]
        public int apartmanId { get; set; }

        public string nazivApartmana { get; set; }

        public int brojSprata { get; set; }

        public bool zauzet { get; set; }

        public int kapacitetOdrasli { get; set; }
        public int kapacitetDeca { get; set; }

        // Izračunati kapacitet (opciono)
        [NotMapped]
        public int ukupniKapacitet => kapacitetOdrasli + kapacitetDeca;

        public List<Rezervacija> listaRezervacija { get; set; } = new List<Rezervacija>();
        public List<CenaApartmana> listaCeneApartmana { get; set; } = new List<CenaApartmana>();
        public List<ApartmanOprema> listaApartmanOprema { get; set; } = new List<ApartmanOprema>();
        public List<ApartmanPopust> listaApartmanPopust { get; set; } = new List<ApartmanPopust>();

        [ForeignKey("zgrada")]
        public int zgradaId { get; set; }
        public Zgrada zgrada { get; set; } // Navigacija ka zgradi

        [ForeignKey("tipApartmana")]
        public int tipApartmanaId { get; set; }
        public TipApartmana tipApartmana { get; set; } // Navigacija ka tipu apartmana
    }
}
