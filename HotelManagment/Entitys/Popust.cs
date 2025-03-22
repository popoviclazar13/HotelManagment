using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Popust
    {
        [Key]
        public int popustId { get; set; }
        [Required]
        public string nazivPopusta { get; set; }
        [Required]
        public double vrednost { get; set; }
        [Required]
        public DateTime pocetniDatum { get; set; }
        [Required]
        public DateTime krajnjiDatum { get; set; }

        public List<ApartmanPopust> listaApartmanPopust { get; set; } = new List<ApartmanPopust>();
    }
}
