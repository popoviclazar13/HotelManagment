using HotelManagment.Entitys;
using HotelManagment.InterfaceRepository;
using HotelManagment.Repository;
using HotelManagment.ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Service
{
    internal class KorisnikService : IKorisnikService
    {
        private readonly IRepository<Korisnik> _korisnikRepository;
        public KorisnikService(IRepository<Korisnik> korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }
        public async Task AddKorisnik(Korisnik korisnik)
        {
            // Provera da li već postoji Korisnik sa istim imenom, telefonom i emailom
            var existingKorisnik = await _korisnikRepository.GetAllAsync();

            // Provera za duplikat ime, telefon i email
            if (existingKorisnik.Any(a => a.imePrezime == korisnik.imePrezime && a.telefon == korisnik.telefon && a.email == korisnik.email))
            {
                throw new Exception("Korisnik sa istim imenom, brojem telefona i email-om već postoji.");
            }

            // Ako ne postoji duplikat, nastavi sa dodavanjem korisnika
            await _korisnikRepository.AddAsync(korisnik);
        }

        public async Task<bool> CanDeleteKorisnikAsync(int korisnikId)
        {
            var rezervacije = await _korisnikRepository.GetAllAsync(); // ✔ Dodaj await
            return !rezervacije.Any(r => r.korisnikId == korisnikId);
        }

        public async Task DeleteKorisnik(int id)
        {
            if (!await CanDeleteKorisnikAsync(id))
            {
                throw new Exception("Ne možete obrisati korisnika koji ima aktivne rezervacije.");
            }

            await _korisnikRepository.DeleteAsync(id);
        }

        public Task<List<Korisnik>> GetAllKorisnik()
        {
            return _korisnikRepository.GetAllAsync();
        }

        public async Task<Korisnik> GetByIdKorisnik(int id)
        {
            return await _korisnikRepository.GetByIdAsync(id);
        }

        public async Task UpdateKorisnik(Korisnik korisnik)
        {
            await _korisnikRepository.UpdateAsync(korisnik);
        }
    }
}
