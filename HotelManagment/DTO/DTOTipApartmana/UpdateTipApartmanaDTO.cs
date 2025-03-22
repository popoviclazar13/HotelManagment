using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOTipApartmana
{
    public class UpdateTipApartmanaDTO
    {
        public int tipApartmanaId { get; set; } // ID tipa apartmana koji se ažurira
        public string nazivTipaApartmana { get; set; }
    }
}
