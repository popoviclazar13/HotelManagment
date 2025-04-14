using HotelManagment.Entitys;
using HotelManagment.InterfaceRepository;
using HotelManagment.Repository;
using HotelManagment.ServiceRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Service
{
    public class RezervacijaUslugaService : IRezervacijaUslugaService
    {
        private readonly IRepository<RezervacijaUsluga> _rezervacijaUslugaRepository;
        private readonly IRepository<Usluga> _uslugaRepository;

        public RezervacijaUslugaService(IRepository<RezervacijaUsluga> rezervacijaUslugaRepository, IRepository<Usluga> uslugaRepository)
        {
            _rezervacijaUslugaRepository = rezervacijaUslugaRepository;
            _uslugaRepository = uslugaRepository;
        }
        public async Task AddRezervacijaUsluga(RezervacijaUsluga rezervacijaUsluga)
        {
            await _rezervacijaUslugaRepository.AddAsync(rezervacijaUsluga);
        }

        public async Task DeleteRezervacijaUsluga(int id)
        {
            await _rezervacijaUslugaRepository.DeleteAsync(id);
        }

        public Task<List<RezervacijaUsluga>> GetAllRezervacijaUsluga()
        {
            return _rezervacijaUslugaRepository.GetAllAsync();
        }

        public async Task<RezervacijaUsluga> GetByIdRezervacijaUsluga(int id)
        {
            return await _rezervacijaUslugaRepository.GetByIdAsync(id);
        }

        public async Task UpdateRezervacijaUsluga(RezervacijaUsluga rezervacijaUsluga)
        {
            await _rezervacijaUslugaRepository.UpdateAsync(rezervacijaUsluga);
        }
        //Posebne metode: 

        public async Task<List<Usluga>> GetUslugeByRezervacijaId(int rezervacijaId)
        {
            // Dohvati sve povezane RezervacijaUsluga zapise za datu rezervaciju
            var rezervacijaUsluge = await _rezervacijaUslugaRepository.GetAllAsync(ru => ru.rezervacijaId == rezervacijaId);

            // Izvuci sve ID-jeve usluga iz dobijenih podataka
            var uslugaIds = rezervacijaUsluge.Select(ru => ru.uslugaId).ToList();

            // Direktno dohvatamo Usluge koje imaju odgovarajuće ID-jeve (optimizovano)
            return await _uslugaRepository.GetAllAsync(u => uslugaIds.Contains(u.uslugaId));
        }

        public async Task<List<RezervacijaUsluga>> GetRezervacijaUslugaByRezervacijaId(int rezervacijaId)
        {
            var rezervacijaUsluge = await _rezervacijaUslugaRepository
            .GetAllAsync(ru => ru.rezervacijaId == rezervacijaId); // Koristimo repo za asinhrono dohvatanje podataka

            return rezervacijaUsluge;
        }
    }
}
