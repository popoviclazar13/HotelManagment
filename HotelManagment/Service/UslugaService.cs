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
    public class UslugaService : IUslugaService
    {
        private readonly IRepository<Usluga> _uslugaRepository;
        public UslugaService(IRepository<Usluga> uslugaRepository)
        {
            _uslugaRepository = uslugaRepository;
        }
        public async Task AddUsluga(Usluga usluga)
        {
            // Provera da li već postoji Usluga sa istim nazivom
            var existingUsluga = await _uslugaRepository.GetAllAsync();
            if (existingUsluga.Any(a => a.nazivUsluge == usluga.nazivUsluge))
            {
                throw new Exception("Apartman sa istim nazivom već postoji.");
            }

            await _uslugaRepository.AddAsync(usluga);
        }

        public async Task<bool> CanDeleteUslugaAsync(int uslugaId)
        {
            var rezervacije = await _uslugaRepository.GetAllAsync(); // ✔ Dodaj await
            return !rezervacije.Any(r => r.uslugaId == uslugaId);
        }

        public async Task DeleteUsluga(int id)
        {
            if (!await CanDeleteUslugaAsync(id))
            {
                throw new Exception("Ne možete obrisati uslugu koja ima aktivne rezervacije.");
            }

            await _uslugaRepository.DeleteAsync(id);
        }

        public Task<List<Usluga>> GetAllUsluga()
        {
            return _uslugaRepository.GetAllAsync();
        }

        public Task<Usluga> GetByIdUsluga(int id)
        {
            return _uslugaRepository.GetByIdAsync(id);
        }

        public async Task UpdateUsluga(Usluga usluga)
        {
            await _uslugaRepository.UpdateAsync(usluga);
        }
    }
}
