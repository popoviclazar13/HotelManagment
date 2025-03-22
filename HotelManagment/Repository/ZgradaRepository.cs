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
    public class ZgradaRepository : IRepository<Zgrada>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Zgrada> _dbSet;

        public ZgradaRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Zgrada>();
        }

        public async Task AddAsync(Zgrada entity)
        {
            await _dbSet.AddAsync(entity);
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

        public async Task<List<Zgrada>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<Zgrada>> GetAllAsync(Expression<Func<Zgrada, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<Zgrada> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(Zgrada entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
