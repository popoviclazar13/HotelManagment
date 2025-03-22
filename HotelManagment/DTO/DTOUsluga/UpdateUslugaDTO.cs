using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOUsluga
{
    public class UpdateUslugaDTO
    {
        public int uslugaId { get; set; } // ID usluge koja se ažurira

        public string nazivUsluge { get; set; }

        public double cenaUsluge { get; set; }
    }
}
