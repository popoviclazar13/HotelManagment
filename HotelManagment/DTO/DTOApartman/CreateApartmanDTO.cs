using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOApartman
{
    public class CreateApartmanDTO
    {
        public string nazivApartmana { get; set; }
        public int brojSprata { get; set; }
        public bool zauzet { get; set; }
        public int kapacitetOdrasli { get; set; }
        public int kapacitetDeca { get; set; }

        public int zgradaId { get; set; }
        public int tipApartmanaId { get; set; }
    }
}
