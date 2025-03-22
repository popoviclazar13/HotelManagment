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
    public class ApartmanRepository : IRepository<Apartman>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<Apartman> _dbSet;

        public ApartmanRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Apartman>();
        }
        public async Task AddAsync(Apartman entity)
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

        public async Task<List<Apartman>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //posebna metoda
        public async Task<List<Apartman>> GetAllAsync(Expression<Func<Apartman, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        //
        public async Task<Apartman> GetByIdAsync(int id)
        {
            return await _dbSet
                    .Include(a => a.listaApartmanOprema)
                        .ThenInclude(ao => ao.oprema) // Učitaj podatke o opremi
                    .Include(a => a.listaCeneApartmana) // Učitaj cene apartmana
                    .Include(a => a.listaRezervacija)
                        .ThenInclude(r => r.korisnik) // Učitaj korisnika rezervacije
                    .Include(a => a.listaRezervacija)
                        .ThenInclude(r => r.agencija) // Učitaj agenciju
                    .Include(a => a.listaApartmanPopust) // Učitaj povezane popuste
                        .ThenInclude(ap => ap.popust) // Učitaj detalje popusta
                    .FirstOrDefaultAsync(a => a.apartmanId == id);
        }

        public async Task UpdateAsync(Apartman entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
