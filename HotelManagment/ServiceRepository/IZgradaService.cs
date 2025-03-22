using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IZgradaService
    {
        Task<List<Zgrada>> GetAllZgrada();
        Task<Zgrada> GetByIdZgrada(int id);
        Task AddZgrada(Zgrada zgrada);
        Task UpdateZgrada(Zgrada zgrada);
        Task DeleteZgrada(int id);
        Task<bool> CanDeleteZgradaAsync(int zgradaId);
    }
}
