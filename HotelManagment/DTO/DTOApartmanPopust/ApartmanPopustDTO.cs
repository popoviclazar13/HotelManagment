using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOApartmanPopust
{
    public class ApartmanPopustDTO
    {
        public int apartmanPopustId { get; set; }

        // Informacije o apartmanu
        public int apartmanId { get; set; }
        public string nazivApartmana { get; set; }

        // Informacije o popustu
        public int popustId { get; set; }
        public string nazivPopusta { get; set; }
        public double vrednost { get; set; }
    }
}
