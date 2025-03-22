using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOPopust
{
    public class CreatePopustDTO
    {
        [Required]
        public string nazivPopusta { get; set; }

        [Required]
        public double vrednost { get; set; }

        [Required]
        public DateTime pocetniDatum { get; set; }

        [Required]
        public DateTime krajnjiDatum { get; set; }
    }
}
