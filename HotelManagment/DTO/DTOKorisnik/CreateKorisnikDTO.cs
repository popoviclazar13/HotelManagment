using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.DTO.DTOKorisnik
{
    public class CreateKorisnikDTO
    {
        public string imePrezime { get; set; }
        public string telefon { get; set; }
        public string email { get; set; }
        public string zemlja { get; set; }
    }
}
