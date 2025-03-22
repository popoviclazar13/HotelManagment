using HotelManagment.DTO.DTOApartmanOprema;
using HotelManagment.DTO.DTOApartmanPopust;
using HotelManagment.DTO.DTOCenaApartmana;
using HotelManagment.DTO.DTORezervacija;
using HotelManagment.DTO.DTOTipApartmana;
using HotelManagment.DTO.DTOZgrada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOApartman
{
    public class ApartmanDTO
    {
        public int apartmanId { get; set; }
        public string nazivApartmana { get; set; }
        public int brojSprata { get; set; }
        public bool zauzet { get; set; }
        public int kapacitetOdrasli { get; set; }
        public int kapacitetDeca { get; set; }

        // Izračunati ukupni kapacitet
        public int ukupniKapacitet { get; set; }

        public ZgradaDTO zgrada { get; set; }
        public TipApartmanaDTO tipApartmana { get; set; }

        // Lista povezanih objekata (ako ih želite prikazivati)
        public List<RezervacijaDTO> listaRezervacija { get; set; } = new List<RezervacijaDTO>();
        public List<CenaApartmanaDTO> listaCeneApartmana { get; set; } = new List<CenaApartmanaDTO>();
        public List<ApartmanOpremaDTO> listaApartmanOprema { get; set; } = new List<ApartmanOpremaDTO>();
        public List<ApartmanPopustDTO> listaApartmanPopust { get; set; } = new List<ApartmanPopustDTO>();
    }
}
