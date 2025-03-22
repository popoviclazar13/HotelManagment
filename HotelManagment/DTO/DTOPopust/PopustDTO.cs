using HotelManagment.DTO.DTOApartmanPopust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOPopust
{
    public class PopustDTO
    {
        public int popustId { get; set; }

        public string nazivPopusta { get; set; }

        public double vrednost { get; set; }

        public DateTime pocetniDatum { get; set; }

        public DateTime krajnjiDatum { get; set; }

        // Lista povezanih apartmana ako je potrebno prikazati
        public List<ApartmanPopustDTO> ListaApartmanPopust { get; set; } = new List<ApartmanPopustDTO>();
    }
}
