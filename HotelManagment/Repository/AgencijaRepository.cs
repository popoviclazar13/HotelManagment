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
    public class AgencijaRepository : IRepository<Agencija>
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<Agencija> _dbSet;

        public AgencijaRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Agencija>();
        }
        public async Task AddAsync(Agencija entity)
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

        public async Task<List<Agencija>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //posebna metoda
        public async Task<List<Agencija>> GetAllAsync(Expression<Func<Agencija, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        //
        public async Task<Agencija> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(Agencija entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
