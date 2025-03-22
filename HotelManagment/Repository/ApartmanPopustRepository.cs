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
    public class ApartmanPopustRepository : IRepository<ApartmanPopust>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<ApartmanPopust> _dbSet;

        public ApartmanPopustRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<ApartmanPopust>();
        }
        public async Task AddAsync(ApartmanPopust entity)
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

        public async Task<List<ApartmanPopust>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //posebna metoda
        public async Task<List<ApartmanPopust>> GetAllAsync(Expression<Func<ApartmanPopust, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        //
        public async Task<ApartmanPopust> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(ApartmanPopust entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
