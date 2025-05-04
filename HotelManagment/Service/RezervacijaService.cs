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
        public async Task<List<RezervacijaPredlog>> GenerisiPredlogeZaOptimizacijuAsync(DateTime datumOd, DateTime datumDo, List<int> odabraniApartmani)
        {
            var sveRezervacije = await _rezervacijaRepository.GetAllAsync(); // Ovde proveri da li Include-uješ Apartman entitet
            var predlozi = new List<RezervacijaPredlog>();

            // Filtriraj rezervacije unutar datuma
            var filtriraneRezervacije = sveRezervacije
                .Where(r => r.pocetniDatum >= datumOd && r.krajnjiDatum <= datumDo)
                .ToList();

            // Grupisanje po apartmanu (samo odabrani)
            var grupisanoPoApartmanu = filtriraneRezervacije
                .Where(r => odabraniApartmani.Contains(r.apartmanId))
                .GroupBy(r => r.apartmanId)
                .ToList();

            foreach (var grupa in grupisanoPoApartmanu)
            {
                var rezervacije = grupa.OrderBy(r => r.pocetniDatum).ToList();

                for (int i = 0; i < rezervacije.Count - 1; i++)
                {
                    var trenutna = rezervacije[i];
                    var sledeca = rezervacije[i + 1];

                    var prazninaPocetak = trenutna.krajnjiDatum;
                    var prazninaKraj = sledeca.pocetniDatum;

                    var razmak = (prazninaKraj - prazninaPocetak).Days;

                    if (razmak >= 1)
                    {
                        // Traži rezervaciju iz DRUGIH apartmana koja može stati u ovu prazninu
                        foreach (var drugaRezervacija in filtriraneRezervacije)
                        {
                            if (drugaRezervacija.apartmanId == trenutna.apartmanId) continue; // Ne ista soba

                            // Bez duplikata i sa validnim kapacitetom
                            if (drugaRezervacija.pocetniDatum >= prazninaPocetak &&
                                drugaRezervacija.krajnjiDatum <= prazninaKraj &&
                                drugaRezervacija.apartman.kapacitetOdrasli <= trenutna.apartman.kapacitetOdrasli) // koristi kapacitet
                            {
                                // Spreči da se ista rezervacija predlaže više puta
                                bool vecPostoji = predlozi.Any(p => p.RezervacijaId == drugaRezervacija.rezervacijaId);
                                if (vecPostoji) continue;

                                predlozi.Add(new RezervacijaPredlog
                                {
                                    RezervacijaId = drugaRezervacija.rezervacijaId,
                                    ApartmanId = drugaRezervacija.apartmanId, // iz kog ide
                                    ApartmanUKojiIde = trenutna.apartmanId,   // u koji ide
                                    StariPocetak = drugaRezervacija.pocetniDatum,
                                    StariKraj = drugaRezervacija.krajnjiDatum,
                                    NoviPocetak = prazninaPocetak,
                                    NoviKraj = prazninaKraj
                                });
                            }
                        }
                    }
                }
            }

            return predlozi;
        }
        /*public async Task<List<RezervacijaPredlog>> GenerisiPredlogeZaOptimizacijuAsync(DateTime datumOd, DateTime datumDo, List<int> odabraniApartmani)
        {
            var sveRezervacije = await _rezervacijaRepository.GetAllAsync();
            var predlozi = new List<RezervacijaPredlog>();

            // Filtriraj sve rezervacije koje su unutar datuma Od - Do
            var filtriraneRezervacije = sveRezervacije
                .Where(r => r.pocetniDatum >= datumOd && r.krajnjiDatum <= datumDo)
                .ToList();

            // Grupisanje rezervacija po apartmanu
            var grupisanoPoApartmanu = filtriraneRezervacije
                .GroupBy(r => r.apartmanId)
                .Where(g => odabraniApartmani.Contains(g.Key))  // Filtriraj samo odabrane apartmane
                .ToList();

            // Prolazak kroz svaki odabrani apartman
            foreach (var grupa in grupisanoPoApartmanu)
            {
                var rezervacije = grupa.OrderBy(r => r.pocetniDatum).ToList();

                // Traženje praznina između rezervacija u ovom apartmanu
                for (int i = 0; i < rezervacije.Count - 1; i++)
                {
                    var trenutna = rezervacije[i];
                    var sledeca = rezervacije[i + 1];

                    var razmak = (sledeca.pocetniDatum - trenutna.krajnjiDatum).Days;

                    // Ako postoji praznina između dve rezervacije
                    if (razmak >= 1)
                    {
                        var prazninaPocetak = trenutna.krajnjiDatum;
                        var prazninaKraj = sledeca.pocetniDatum;

                        // Sada tražimo rezervaciju iz drugih apartmana koja bi mogla da se stavi u ovu prazninu
                        foreach (var drugiApartman in filtriraneRezervacije.Where(r => r.apartmanId != trenutna.apartmanId))
                        {
                            // Provera da li kapacitet odgovara
                            if (drugiApartman.apartman.kapacitetOdrasli <= (sledeca.apartman.kapacitetOdrasli - trenutna.apartman.kapacitetOdrasli) &&
                                drugiApartman.pocetniDatum >= prazninaPocetak &&
                                drugiApartman.krajnjiDatum <= prazninaKraj)
                            {
                                predlozi.Add(new RezervacijaPredlog
                                {
                                    RezervacijaId = drugiApartman.rezervacijaId,
                                    ApartmanId = trenutna.apartmanId,  // Premesti rezervaciju u trenutni apartman
                                    StariPocetak = drugiApartman.pocetniDatum,
                                    StariKraj = drugiApartman.krajnjiDatum,
                                    NoviPocetak = prazninaPocetak,
                                    NoviKraj = prazninaKraj
                                });
                            }
                        }
                    }
                }
            }

            return predlozi;
        }*/

        public async Task<List<Rezervacija>> GetRezervacijaByKorisnikId(int id)
        {
            return await _rezervacijaRepository.GetAllAsync(r => r.korisnikId == id);
        }
    }
}
