using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IOpremaService
    {
        Task<List<Oprema>> GetAllOprema();
        Task<Oprema> GetByIdOprema(int id);
        Task AddOprema(Oprema oprema);
        Task UpdateOprema(Oprema oprema);
        Task DeleteOprema(int id);
        Task<bool> CanDeleteOpremaAsync(int opremaId);
    }
}
