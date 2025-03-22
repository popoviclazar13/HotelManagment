using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOApartmanOprema
{
    public class ApartmanOpremaDTO
    {
        public int apartmanOpremaId { get; set; }

        // Informacije o apartmanu
        public int apartmanId { get; set; }
        public string nazivApartmana { get; set; }

        // Informacije o opremi
        public int opremaId { get; set; }
        public string nazivOpreme { get; set; }

        public int kolicinaOpreme { get; set; }
    }
}
