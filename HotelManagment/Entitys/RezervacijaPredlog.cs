using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class RezervacijaPredlog
    {
        public int RezervacijaId { get; set; }
        public int ApartmanId { get; set; }
        public int ApartmanUKojiIde { get; set; }

        public DateTime StariPocetak { get; set; }
        public DateTime StariKraj { get; set; }

        public DateTime NoviPocetak { get; set; }
        public DateTime NoviKraj { get; set; }
    }
}
