using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Oprema
    {
        [Key]
        public int opremaId { get; set; }
        [Required]
        public string nazivOprema { get; set; }

        public List<ApartmanOprema> listaApartmanOprema { get; set; } = new List<ApartmanOprema>();
    }
}
