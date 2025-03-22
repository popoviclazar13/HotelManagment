using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IPopustService
    {
        Task<List<Popust>> GetAllPopust();
        Task<Popust> GetByIdPopust(int id);
        Task AddPopust(Popust popust);
        Task UpdatePopust(Popust popust);
        Task DeletePopust(int id);
        Task<bool> CanDeletePopustAsync(int popustId);
    }
}
