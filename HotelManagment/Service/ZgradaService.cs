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
    public class ZgradaService : IZgradaService
    {
        private readonly IRepository<Zgrada> _zgradaRepository;
        public ZgradaService(IRepository<Zgrada> zgradaRepository)
        {
            _zgradaRepository = zgradaRepository;
        }

        public async Task AddZgrada(Zgrada zgrada)
        {
            // Provera da li već postoji Zgrada sa istim nazivom
            var existingZgradai = await _zgradaRepository.GetAllAsync();
            if (existingZgradai.Any(a => a.naziv == zgrada.naziv))
            {
                throw new Exception("Zgrada sa istim nazivom već postoji.");
            }

            await _zgradaRepository.AddAsync(zgrada);
        }

        public Task<bool> CanDeleteZgradaAsync(int zgradaId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteZgrada(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Zgrada>> GetAllZgrada()
        {
            return _zgradaRepository.GetAllAsync();
        }

        public async Task<Zgrada> GetByIdZgrada(int id)
        {
            return await _zgradaRepository.GetByIdAsync(id);
        }

        public async Task UpdateZgrada(Zgrada zgrada)
        {
            await _zgradaRepository.UpdateAsync(zgrada);
        }
    }
}
