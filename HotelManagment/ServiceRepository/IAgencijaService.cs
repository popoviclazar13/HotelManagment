using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IAgencijaService
    {
        Task<List<Agencija>> GetAllAgencija();
        Task<Agencija> GetByIdAgencija(int id);
        Task AddAgencija(Agencija agencija);
        Task UpdateAgencija(Agencija agencija);
        Task DeleteAgencija(int id);
        Task<bool> CanDeleteAgencijaAsync(int agencijaId);
    }
}
