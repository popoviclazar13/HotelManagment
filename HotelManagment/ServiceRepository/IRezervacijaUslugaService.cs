using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IRezervacijaUslugaService
    {
        Task<List<RezervacijaUsluga>> GetAllRezervacijaUsluga();
        Task<RezervacijaUsluga> GetByIdRezervacijaUsluga(int id);
        Task AddRezervacijaUsluga(RezervacijaUsluga rezervacijaUsluga);
        Task UpdateRezervacijaUsluga(RezervacijaUsluga rezervacijaUsluga);
        Task DeleteRezervacijaUsluga(int id);
    }
}
