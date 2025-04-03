using HotelManagment.Entitys;
using HotelManagment.InterfaceRepository;
using HotelManagment.Repository;
using HotelManagment.ServiceRepository;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Service
{
    public class RezervacijaService : IRezervacijaService
    {
        private readonly IRepository<Rezervacija> _rezervacijaRepository;
        public RezervacijaService(IRepository<Rezervacija> rezervacijaRepository)
        {
            _rezervacijaRepository = rezervacijaRepository;
        }
        public async Task AddRezervacija(Rezervacija rezervacija)
        {
            if (!await IsApartmentAvailable(rezervacija.apartmanId, rezervacija.pocetniDatum, rezervacija.krajnjiDatum))
            {
                throw new InvalidOperationException("Apartman nije dostupan u izabranom terminu.");
            }

            await _rezervacijaRepository.AddAsync(rezervacija);
        }

        public async Task<bool> CanDeleteRezervacijaAsync(int rezervacijaId)
        {
            var rezervacija = await _rezervacijaRepository.GetByIdAsync(rezervacijaId);
            if (rezervacija == null)
            {
                throw new KeyNotFoundException("Rezervacija nije pronađena.");
            }

            DateTime today = DateTime.Today;

            // Ako rezervacija trenutno traje, nije moguće brisanje
            return !(rezervacija.pocetniDatum <= today && rezervacija.krajnjiDatum >= today);
        }

        public async Task DeleteRezervacija(int id)
        {
            var rezervacija = await _rezervacijaRepository.GetByIdAsync(id);
            if (rezervacija == null)
            {
                throw new KeyNotFoundException("Rezervacija nije pronađena.");
            }

            if (!await CanDeleteRezervacijaAsync(id))
            {
                throw new InvalidOperationException("Rezervaciju nije moguće obrisati.");
            }

            await _rezervacijaRepository.DeleteAsync(id);
        }

        public async Task<List<Rezervacija>> GetAllRezervacija()
        {
            return await _rezervacijaRepository.GetAllAsync();
        }

        public async Task<Rezervacija> GetByIdRezervacija(int id)
        {
            var rezervacija = await _rezervacijaRepository.GetByIdAsync(id);
            if (rezervacija == null)
            {
                throw new KeyNotFoundException("Rezervacija nije pronađena.");
            }
            return rezervacija;
        }

        public async Task UpdateRezervacija(Rezervacija rezervacija)
        {
            if (!await IsApartmentAvailableForUpdate(rezervacija.rezervacijaId, rezervacija.apartmanId, rezervacija.pocetniDatum, rezervacija.krajnjiDatum))
            {
                throw new InvalidOperationException("Apartman nije dostupan u izabranom terminu.");
            }

            await _rezervacijaRepository.UpdateAsync(rezervacija);
        }

        public async Task<bool> IsApartmentAvailable(int apartmanId, DateTime startDate, DateTime endDate)
        {
            var existingReservations = await _rezervacijaRepository.GetAllAsync();

            return !existingReservations.Any(r =>
                r.apartmanId == apartmanId &&
                r.pocetniDatum < endDate &&  // Početak postojeće rezervacije mora biti pre kraja nove
                r.krajnjiDatum > startDate    // Kraj postojeće rezervacije mora biti posle početka nove
            );
        }
        public async Task<bool> IsApartmentAvailableForUpdate(int rezervacijaId, int apartmanId, DateTime startDate, DateTime endDate)
        {
            var existingReservations = await _rezervacijaRepository.GetAllAsync();

            return !existingReservations.Any(r =>
                r.rezervacijaId != rezervacijaId &&  // Ignoriše trenutnu rezervaciju
                r.apartmanId == apartmanId &&
                r.pocetniDatum < endDate &&
                r.krajnjiDatum > startDate
            );
        }

        async Task<List<Rezervacija>> IRezervacijaService.GetRezervacijeByApartmanId(int id)
        {
            return await _rezervacijaRepository.GetAllAsync(r => r.apartmanId == id);
        }

        public async Task<List<Rezervacija>> GetRezervacijeByPocetniDatum(DateTime pocetniDatum)
        {
            // Pretpostavljamo da imamo repository metod koji vraća rezervacije koje počinju na datom datumu
            return await _rezervacijaRepository.GetAllAsync(r => r.pocetniDatum.Date == pocetniDatum.Date);
        }

        public async Task<List<Rezervacija>> GetRezervacijeByKrajnjiDatum(DateTime krajnjiDatum)
        {
            // Pretpostavljamo da imamo repository metod koji vraća rezervacije koje se završavaju na datom datumu
            return await _rezervacijaRepository.GetAllAsync(r => r.krajnjiDatum.Date == krajnjiDatum.Date);
        }
    }
}
