using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOZgrada
{
    public class UpdateZgradaDTO
    {
        public int zgradaId { get; set; }  // ID se koristi za identifikaciju zgrade koju ažuriramo
        public string naziv { get; set; }
    }
}
