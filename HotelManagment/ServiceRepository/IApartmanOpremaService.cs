using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IApartmanOpremaService
    {
        Task<List<ApartmanOprema>> GetAllApartmanOprema();
        Task<ApartmanOprema> GetByIdApartmanOprema(int id);
        Task AddApartmanOprema(ApartmanOprema apartmanOprema);
        Task UpdateApartmanOprema(ApartmanOprema apartmanOprema);
        Task DeleteApartmanOprema(int id);
    }
}
