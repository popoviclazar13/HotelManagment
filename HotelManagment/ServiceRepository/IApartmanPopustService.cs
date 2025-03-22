using HotelManagment.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.ServiceRepository
{
    public interface IApartmanPopustService
    {
        Task<List<ApartmanPopust>> GetAllApartmanPopust();
        Task<ApartmanPopust> GetByIdApartmanPopust(int id);
        Task AddApartmanPopust(ApartmanPopust apartmanPopust);
        Task UpdateApartmanPopust(ApartmanPopust apartmanPopust);
        Task DeleteApartmanPopust(int id);
    }
}
