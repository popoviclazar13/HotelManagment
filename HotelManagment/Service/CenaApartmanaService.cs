using HotelManagment.Entitys;
using HotelManagment.InterfaceRepository;
using HotelManagment.ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Service
{
    public class CenaApartmanaService : ICenaApartmanaService
    {
        private readonly IRepository<CenaApartmana> _cenaApartmanaRepository;
        public CenaApartmanaService(IRepository<CenaApartmana> cenaApartmanaRepository)
        {
            _cenaApartmanaRepository = cenaApartmanaRepository;
        }
        public async Task AddCenaApartmana(CenaApartmana cenaApartmana)
        {
            if (!await IsValidDateRange(cenaApartmana.apartmanId, cenaApartmana.pocetniDatum, cenaApartmana.krajnjiDatum))
            {
                throw new InvalidOperationException("Apartman već ima postavljenu cenu u izabranom terminu.");
            }

            await _cenaApartmanaRepository.AddAsync(cenaApartmana);
        }

        public async Task<bool> CanDeleteCenaApartmanaAsync(int cenaApartmanaId)
        {
            var cenaApartmana = await _cenaApartmanaRepository.GetByIdAsync(cenaApartmanaId);
            if (cenaApartmana == null)
            {
                throw new KeyNotFoundException("Cena apartmana nije pronađena.");
            }

            DateTime today = DateTime.Today;

            // Ako cena važi za trenutni datum, nije moguće brisanje
            return !(cenaApartmana.pocetniDatum <= today && cenaApartmana.krajnjiDatum >= today);
        }

        public async Task DeleteCenaApartmana(int id)
        {
            var cenaApartmana = await _cenaApartmanaRepository.GetByIdAsync(id);
            if (cenaApartmana == null)
            {
                throw new KeyNotFoundException("Cena apartmana nije pronađena.");
            }

            if (!await CanDeleteCenaApartmanaAsync(id))
            {
                throw new InvalidOperationException("Cenu apartmana nije moguće obrisati.");
            }

            await _cenaApartmanaRepository.DeleteAsync(id);
        }

        public async Task<List<CenaApartmana>> GetAllCenaApartmana()
        {
            return await _cenaApartmanaRepository.GetAllAsync();
        }

        public async Task<CenaApartmana> GetByIdCenaApartmana(int id)
        {
            var cenaApartmana = await _cenaApartmanaRepository.GetByIdAsync(id);
            if (cenaApartmana == null)
            {
                throw new KeyNotFoundException("Cena apartmana nije pronađena.");
            }
            return cenaApartmana;
        }

        public async Task UpdateCenaApartmana(CenaApartmana cenaApartmana)
        {
            var existingCena = await _cenaApartmanaRepository.GetByIdAsync(cenaApartmana.cenaApartmanaId);
            if (existingCena == null)
            {
                throw new KeyNotFoundException("Cena apartmana nije pronađena.");
            }

            if (!await IsValidDateRangeForUpdate(cenaApartmana))
            {
                throw new InvalidOperationException("Apartman već ima postavljenu cenu u izabranom terminu.");
            }

            await _cenaApartmanaRepository.UpdateAsync(cenaApartmana);
        }
        private async Task<bool> IsValidDateRange(int apartmanId, DateTime pocetniDatum, DateTime krajnjiDatum)
        {
            if (pocetniDatum >= krajnjiDatum)
            {
                return false; // Početni datum mora biti pre krajnjeg
            }

            var existingCene = await _cenaApartmanaRepository.GetAllAsync();

            // Provera da li postoji neka cena koja se preklapa sa novom cenom
            bool postojiPreklapanje = existingCene.Any(c =>
                c.apartmanId == apartmanId && // Proveravamo samo za isti apartman
                !(c.krajnjiDatum <= pocetniDatum || c.pocetniDatum >= krajnjiDatum) // Provera preklapanja datuma
            );

            return !postojiPreklapanje;
        }
        private async Task<bool> IsValidDateRangeForUpdate(CenaApartmana cenaApartmana)
        {
            if (cenaApartmana.pocetniDatum >= cenaApartmana.krajnjiDatum)
            {
                return false; // Početni datum mora biti pre krajnjeg
            }

            var existingCene = await _cenaApartmanaRepository.GetAllAsync();

            bool postojiPreklapanje = existingCene.Any(c =>
                c.apartmanId == cenaApartmana.apartmanId && // Isti apartman
                c.cenaApartmanaId != cenaApartmana.cenaApartmanaId && // Ignorisanje trenutnog zapisa
                !(c.krajnjiDatum <= cenaApartmana.pocetniDatum || c.pocetniDatum >= cenaApartmana.krajnjiDatum) // Provera preklapanja
            );

            return !postojiPreklapanje;
        }
    }
}
