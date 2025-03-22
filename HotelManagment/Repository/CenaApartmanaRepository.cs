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
    public class CenaApartmanaRepository : IRepository<CenaApartmana>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<CenaApartmana> _dbSet;

        public CenaApartmanaRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<CenaApartmana>();
        }

        public async Task AddAsync(CenaApartmana entity)
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

        public async Task<List<CenaApartmana>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //posebna metoda
        public async Task<List<CenaApartmana>> GetAllAsync(Expression<Func<CenaApartmana, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<CenaApartmana> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(CenaApartmana entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
