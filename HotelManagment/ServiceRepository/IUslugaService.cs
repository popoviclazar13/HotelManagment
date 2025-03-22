using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IUslugaService
    {
        Task<List<Usluga>> GetAllUsluga();
        Task<Usluga> GetByIdUsluga(int id);
        Task AddUsluga(Usluga usluga);
        Task UpdateUsluga(Usluga usluga);
        Task DeleteUsluga(int id);
        Task<bool> CanDeleteUslugaAsync(int uslugaId);
    }
}
