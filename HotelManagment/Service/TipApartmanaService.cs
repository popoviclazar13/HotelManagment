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
    public class TipApartmanaService : ITipApartmanaService
    {
        private readonly IRepository<TipApartmana> _tipApartmanaRepository;
        public TipApartmanaService(IRepository<TipApartmana> tipApartmanaRepository)
        {
            _tipApartmanaRepository = tipApartmanaRepository;
        }
        public async Task AddTipApartmana(TipApartmana tipApartmana)
        {
            // Provera da li već postoji TipApartmana sa istim nazivom
            var existingTipApartmanai = await _tipApartmanaRepository.GetAllAsync();
            if (existingTipApartmanai.Any(a => a.nazivTipaApartmana == tipApartmana.nazivTipaApartmana))
            {
                throw new Exception("TipApartmana sa istim nazivom već postoji.");
            }

            await _tipApartmanaRepository.AddAsync(tipApartmana);
        }

        public Task<bool> CanDeleteTipApartmanaAsync(int tipApartmanaId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTipApartmana(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TipApartmana>> GetAllTipApartmana()
        {
            return _tipApartmanaRepository.GetAllAsync();
        }

        public async Task<TipApartmana> GetByIdTipApartmana(int id)
        {
            return await _tipApartmanaRepository.GetByIdAsync(id);
        }

        public async Task UpdateTipApartmana(TipApartmana tipApartmana)
        {
            await _tipApartmanaRepository.UpdateAsync(tipApartmana);
        }
    }
}
