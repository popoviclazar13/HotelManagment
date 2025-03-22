using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class Zgrada
    {
        [Key]
        public int zgradaId { get; set; }
        public string naziv { get; set; }

        public List<Apartman> listaApartmana { get; set; } = new List<Apartman>();
    }
}
