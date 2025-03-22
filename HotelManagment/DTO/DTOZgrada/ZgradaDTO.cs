using HotelManagment.DTO.DTOApartman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOZgrada
{
    public class ZgradaDTO
    {
        public int zgradaId { get; set; }
        public string naziv { get; set; }

        // Lista apartmana, ako želite prikazivati povezane apartmane
        public List<ApartmanDTO> listaApartmana { get; set; } = new List<ApartmanDTO>();
    }
}
