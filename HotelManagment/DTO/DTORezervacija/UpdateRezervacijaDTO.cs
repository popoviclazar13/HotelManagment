using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTORezervacija
{
    public class UpdateRezervacijaDTO
    {
        public int RezervacijaId { get; set; }  // Neophodno za identifikaciju entiteta koji se ažurira
        public DateTime? PocetniDatum { get; set; }  // Možda samo ako želite da ažurirate
        public DateTime? KrajnjiDatum { get; set; }
        public int? BrojGostiju { get; set; }
        public double? UkupnaCena { get; set; }
        public double? CenaKonacna { get; set; }
        public string Komentar { get; set; }
        public bool? Placeno { get; set; }
        public string? nacinPlacanja { get; set; }

        // Povezani podaci
        public int? ApartmanId { get; set; }
        public int? KorisnikId { get; set; }
        public int? AgencijaId { get; set; }
    }
}
