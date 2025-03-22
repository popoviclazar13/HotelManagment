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
    public class RezervacijaUslugaRepository : IRepository<RezervacijaUsluga>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<RezervacijaUsluga> _dbSet;

        public RezervacijaUslugaRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<RezervacijaUsluga>();
        }
        public async Task AddAsync(RezervacijaUsluga entity)
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

        public async Task<List<RezervacijaUsluga>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //posebna metoda
        public async Task<List<RezervacijaUsluga>> GetAllAsync(Expression<Func<RezervacijaUsluga, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        //
        public async Task<RezervacijaUsluga> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(RezervacijaUsluga entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
