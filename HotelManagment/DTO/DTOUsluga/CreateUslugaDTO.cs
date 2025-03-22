using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOUsluga
{
    public class CreateUslugaDTO
    {
        [Required]
        public string nazivUsluge { get; set; }

        [Required]
        public double cenaUsluge { get; set; }
    }
}
