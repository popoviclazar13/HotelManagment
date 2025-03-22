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
    public class KorisnikRepository : IRepository<Korisnik>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<Korisnik> _dbSet;
        public KorisnikRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Korisnik>();
        }
        public async Task AddAsync(Korisnik entity)
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

        public async Task<List<Korisnik>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //posebna metoda
        public async Task<List<Korisnik>> GetAllAsync(Expression<Func<Korisnik, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Korisnik> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(Korisnik entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
