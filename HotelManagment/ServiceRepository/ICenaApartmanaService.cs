using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface ICenaApartmanaService
    {
        Task<List<CenaApartmana>> GetAllCenaApartmana();
        Task<CenaApartmana> GetByIdCenaApartmana(int id);
        Task AddCenaApartmana(CenaApartmana cenaApartmana);
        Task UpdateCenaApartmana(CenaApartmana cenaApartmana);
        Task DeleteCenaApartmana(int id);
        Task<bool> CanDeleteCenaApartmanaAsync(int cenaApartmanaId);
    }
}
