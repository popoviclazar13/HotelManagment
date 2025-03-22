using HotelManagment.DTO.DTOApartman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOTipApartmana
{
    public class TipApartmanaDTO
    {
        public int tipApartmanaId { get; set; }
        public string nazivTipaApartmana { get; set; }

        // Lista apartmana ovog tipa (ako je potrebno da se prikazuju)
        public List<ApartmanDTO> listaApartmana { get; set; } = new List<ApartmanDTO>();
    }
}
