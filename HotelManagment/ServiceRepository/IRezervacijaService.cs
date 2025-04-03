using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IRezervacijaService
    {
        Task<List<Rezervacija>> GetAllRezervacija();
        Task<Rezervacija> GetByIdRezervacija(int id);
        Task AddRezervacija(Rezervacija rezervacija);
        Task UpdateRezervacija(Rezervacija rezervacija);
        Task DeleteRezervacija(int id);
        Task<bool> CanDeleteRezervacijaAsync(int rezervacijaId);
        Task<List<Rezervacija>> GetRezervacijeByApartmanId(int id);
        Task<List<Rezervacija>> GetRezervacijeByPocetniDatum(DateTime pocetniDatum);
        Task<List<Rezervacija>> GetRezervacijeByKrajnjiDatum(DateTime krajnjiDatum);
    }
}
