using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Agencija
    {
        [Key]
        public int agencijaId { get; set; }

        [Required]
        public string nazivAgencije { get; set; }

        // Lista rezervacija preko agencije
        public List<Rezervacija> listaRezervacija { get; set; } = new List<Rezervacija>();
    }
}
