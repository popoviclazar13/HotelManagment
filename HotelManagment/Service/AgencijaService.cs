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
    public class AgencijaService : IAgencijaService
    {
        private readonly IRepository<Agencija> _agencijaRepository;
        public AgencijaService(IRepository<Agencija> agencijaRepository)
        {
            _agencijaRepository = agencijaRepository;
        }
        public async Task AddAgencija(Agencija agencija)
        {
            // Provera da li već postoji agencija sa istim nazivom
            var existingAgencija = await _agencijaRepository.GetAllAsync();
            if (existingAgencija.Any(a => a.nazivAgencije == agencija.nazivAgencije))
            {
                throw new Exception("Apartman sa istim nazivom već postoji.");
            }

            await _agencijaRepository.AddAsync(agencija);
        }

        public async Task<bool> CanDeleteAgencijaAsync(int agencijaId)
        {
            var rezervacije = await _agencijaRepository.GetAllAsync(); // ✔ Dodaj await
            return !rezervacije.Any(r => r.agencijaId == agencijaId);
        }

        public async Task DeleteAgencija(int id)
        {
            if (!await CanDeleteAgencijaAsync(id))
            {
                throw new Exception("Ne možete obrisati agenciju koji ima aktivne rezervacije.");
            }

            await _agencijaRepository.DeleteAsync(id);
        }

        public Task<List<Agencija>> GetAllAgencija()
        {
            return _agencijaRepository.GetAllAsync();
        }

        public async Task<Agencija> GetByIdAgencija(int id)
        {
            return await _agencijaRepository.GetByIdAsync(id);
        }

        public async Task UpdateAgencija(Agencija agencija)
        {
            await _agencijaRepository.UpdateAsync(agencija);
        }
    }
}
