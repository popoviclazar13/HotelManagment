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
    public class ApartmanService : IApartmanService
    {
        private readonly IRepository<Apartman> _apartmanRepository;
        private readonly IRepository<Rezervacija> _rezervacijaRepository;
        public ApartmanService(IRepository<Apartman> apartmanRepository, IRepository<Rezervacija> rezervacijaRepository)
        {
            _apartmanRepository = apartmanRepository;
            _rezervacijaRepository = rezervacijaRepository;
        }
        public async Task AddApartman(Apartman apartman)
        {
            // Provera da li već postoji apartman sa istim nazivom
            var existingApartmani = await _apartmanRepository.GetAllAsync();
            if (existingApartmani.Any(a => a.nazivApartmana == apartman.nazivApartmana))
            {
                throw new Exception("Apartman sa istim nazivom već postoji.");
            }

            await _apartmanRepository.AddAsync(apartman);
        }

        public async Task<bool> CanDeleteApartmanAsync(int apartmanId)
        {
            var rezervacije = await _rezervacijaRepository.GetAllAsync(); // ✔ Dodaj await
            return !rezervacije.Any(r => r.apartmanId == apartmanId);
        }

        public async Task DeleteApartman(int id)
        {
            if (!await CanDeleteApartmanAsync(id))
            {
                throw new Exception("Ne možete obrisati apartman koji ima aktivne rezervacije.");
            }

            await _apartmanRepository.DeleteAsync(id);
        }

        public Task<List<Apartman>> GetAllApartman()
        {
            return _apartmanRepository.GetAllAsync();
        }

        public async Task<Apartman> GetByIdApartman(int id)
        {
            return await _apartmanRepository.GetByIdAsync(id);
        }

        public async Task UpdateApartman(Apartman apartman)
        {
            await _apartmanRepository.UpdateAsync(apartman);
        }

        public async Task<List<Apartman>> GetApartmaniByZgradaId(int zgradaId)
        {
            var apartmani = await _apartmanRepository.GetAllAsync();
            return apartmani.Where(a => a.zgradaId == zgradaId).ToList();
        }
    }
}
