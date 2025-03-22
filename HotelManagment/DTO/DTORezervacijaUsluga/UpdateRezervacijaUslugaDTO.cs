using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTORezervacijaUsluga
{
    public class UpdateRezervacijaUslugaDTO
    {
        public int RezervacijaUslugaId { get; set; }
        public int? RezervacijaId { get; set; }  // Opcionalno, može se promeniti
        public int? UslugaId { get; set; }  // Opcionalno, može se promeniti
        public int? Kolicina { get; set; }
        public DateTime? Datum { get; set; }
    }
}
