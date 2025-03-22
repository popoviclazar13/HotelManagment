using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IKorisnikService
    {
        Task<List<Korisnik>> GetAllKorisnik();
        Task<Korisnik> GetByIdKorisnik(int id);
        Task AddKorisnik(Korisnik korisnik);
        Task UpdateKorisnik(Korisnik korisnik);
        Task DeleteKorisnik(int id);
        Task<bool> CanDeleteKorisnikAsync(int korisnikId);
    }
}
