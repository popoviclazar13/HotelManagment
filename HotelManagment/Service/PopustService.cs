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
    public class PopustService : IPopustService
    {
        private readonly IRepository<Popust> _popustRepository;
        public PopustService(IRepository<Popust> popustRepository)
        {
            _popustRepository = popustRepository;
        }

        public async Task AddPopust(Popust popust)
        {
            // Provera da li već postoji Popust sa istim nazivom
            var existingPopusti = await _popustRepository.GetAllAsync();
            if (existingPopusti.Any(a => a.nazivPopusta == popust.nazivPopusta))
            {
                throw new Exception("Popust sa istim nazivom već postoji.");
            }

            await _popustRepository.AddAsync(popust);
        }

        public Task<bool> CanDeletePopustAsync(int popustId)
        {
            throw new NotImplementedException();
        }

        public Task DeletePopust(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Popust>> GetAllPopust()
        {
            return _popustRepository.GetAllAsync();
        }

        public async Task<Popust> GetByIdPopust(int id)
        {
            return await _popustRepository.GetByIdAsync(id);
        }

        public async Task UpdatePopust(Popust popust)
        {
            await _popustRepository.UpdateAsync(popust);
        }
    }
}
