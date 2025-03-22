using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IApartmanService
    {
        Task<List<Apartman>> GetAllApartman();
        Task<Apartman> GetByIdApartman(int id);
        Task AddApartman(Apartman apartman);
        Task UpdateApartman(Apartman apartman);
        Task DeleteApartman(int id);
        Task<bool> CanDeleteApartmanAsync(int apartmanId);
        Task<List<Apartman>> GetApartmaniByZgradaId(int zgradaId);
    }
}
