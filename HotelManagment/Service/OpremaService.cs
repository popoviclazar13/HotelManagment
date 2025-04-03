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
    public class OpremaService : IOpremaService
    {
        private readonly IRepository<Oprema> _opremaRepository;
        public OpremaService(IRepository<Oprema> opremaRepository)
        {
            _opremaRepository = opremaRepository;
        }
        public async Task AddOprema(Oprema oprema)
        {
            // Provera da li već postoji Oprema sa istim nazivom
            var existingOpremai = await _opremaRepository.GetAllAsync();
            if (existingOpremai.Any(a => a.nazivOprema == oprema.nazivOprema))
            {
                throw new Exception("Oprema sa istim nazivom već postoji.");
            }

            await _opremaRepository.AddAsync(oprema);
        }

        public Task<bool> CanDeleteOpremaAsync(int opremaId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOprema(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Oprema>> GetAllOprema()
        {
            return _opremaRepository.GetAllAsync();
        }

        public async Task<Oprema> GetByIdOprema(int id)
        {
            return await _opremaRepository.GetByIdAsync(id);
        }

        public async Task UpdateOprema(Oprema oprema)
        {
            // Provera da li već postoji Oprema sa istim nazivom, ali različitim ID-jem
            var existingOpremai = await _opremaRepository.GetAllAsync();
            if (existingOpremai.Any(a => a.nazivOprema == oprema.nazivOprema && a.opremaId != oprema.opremaId))
            {
                throw new Exception("Oprema sa istim nazivom već postoji.");
            }

            // Ažuriranje opreme
            await _opremaRepository.UpdateAsync(oprema);
        }
    }
}
