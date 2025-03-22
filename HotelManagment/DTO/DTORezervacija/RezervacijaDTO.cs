using HotelManagment.DTO.DTORezervacijaUsluga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTORezervacija
{
    public class RezervacijaDTO
    {
        public int RezervacijaId { get; set; }
        public DateTime PocetniDatum { get; set; }
        public DateTime KrajnjiDatum { get; set; }
        public int BrojGostiju { get; set; }
        public double UkupnaCena { get; set; }
        public double CenaKonacna { get; set; }
        public string Komentar { get; set; }
        public bool Placeno { get; set; }
        public string nacinPlacanja { get; set; }

        // Povezani podaci
        public int ApartmanId { get; set; }
        public int KorisnikId { get; set; }
        public int? AgencijaId { get; set; }

        // Lista usluga
        public List<RezervacijaUslugaDTO> ListaRezervacijaUsluga { get; set; } = new List<RezervacijaUslugaDTO>();
    }
}
