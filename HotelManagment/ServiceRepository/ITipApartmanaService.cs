using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface ITipApartmanaService
    {
        Task<List<TipApartmana>> GetAllTipApartmana();
        Task<TipApartmana> GetByIdTipApartmana(int id);
        Task AddTipApartmana(TipApartmana tipApartmana);
        Task UpdateTipApartmana(TipApartmana tipApartmana);
        Task DeleteTipApartmana(int id);
        Task<bool> CanDeleteTipApartmanaAsync(int tipApartmanaId);
    }
}
