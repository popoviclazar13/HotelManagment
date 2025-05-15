using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Entitys
{
    public class KalendarViewModel
    {
        public int BrojSprata { get; set; }
        public string Naziv { get; set; }
        public int Kapacitet { get; set; }

        // Rezervacije po datumu (DateTime), može da pokrije sve potrebne dane u 4 meseca
        public Dictionary<string, string> RezervacijePoDatumu { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> JeKrajRezervacijePoDatumu { get; set; } = new Dictionary<string, bool>();
    }
}
