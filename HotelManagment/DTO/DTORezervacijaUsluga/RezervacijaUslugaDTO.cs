using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTORezervacijaUsluga
{
    public class RezervacijaUslugaDTO
    {
        public int RezervacijaUslugaId { get; set; }
        public int RezervacijaId { get; set; }
        public int UslugaId { get; set; }
        public int Kolicina { get; set; }
        public DateTime Datum { get; set; }
        public string NazivUsluge { get; set; }  // Opcionalno, naziv usluge
    }
}
