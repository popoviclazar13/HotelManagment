using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOCenaApartmana
{
    public class CenaApartmanaDTO
    {
        public int cenaApartmanaId { get; set; }

        // Informacije o apartmanu
        public int apartmanId { get; set; }
        public string nazivApartmana { get; set; }

        public DateTime pocetniDatum { get; set; }
        public DateTime krajnjiDatum { get; set; }
        public double cenaPoOsobi { get; set; }
    }
}
