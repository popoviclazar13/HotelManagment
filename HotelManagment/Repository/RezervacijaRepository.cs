using HotelManagment.Data;
using HotelManagment.Entitys;
using HotelManagment.InterfaceRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagment.Repository
{
    public class RezervacijaRepository : IRepository<Rezervacija>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<Rezervacija> _dbSet;

        public RezervacijaRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Rezervacija>();
        }
        public async Task AddAsync(Rezervacija entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Rezervacija>> GetAllAsync()
        {
            return await _context.Rezervacije
                .Include(r => r.korisnik)
                .Include(r => r.agencija)
                .Include(r => r.apartman) // Može biti korisno ako treba dodatne informacije o apartmanu
                .ToListAsync();
        }
        //posebna metoda
        public async Task<List<Rezervacija>> GetAllAsync(Expression<Func<Rezervacija, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Rezervacija> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(Rezervacija entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Rezervacija>> GetRezervacijeByApartmanId(int id)
        {
            return await _context.Rezervacije
                                 .Where(r => r.apartmanId == id)
                                 .ToListAsync();
        }
    }
}
