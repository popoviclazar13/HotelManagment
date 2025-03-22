using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOApartmanOprema
{
    public class UpdateApartmanOpremaDTO
    {
        public int apartmanOpremaId { get; set; }
        public int apartmanId { get; set; }
        public int opremaId { get; set; }
        public int kolicinaOpreme { get; set; }
    }
}
