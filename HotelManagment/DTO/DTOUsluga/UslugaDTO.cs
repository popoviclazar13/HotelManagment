using HotelManagment.DTO.DTORezervacijaUsluga;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOUsluga
{
    public class UslugaDTO
    {
        public int uslugaId { get; set; }

        public string nazivUsluge { get; set; }

        public double cenaUsluge { get; set; }

        // Lista povezanih rezervacija kroz RezervacijaUsluga (ako želite prikazivati ove podatke)
        public List<RezervacijaUslugaDTO> ListaRezervacijaUsluga { get; set; } = new List<RezervacijaUslugaDTO>();
    }
}
