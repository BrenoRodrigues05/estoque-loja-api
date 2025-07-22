
using APILojaEstoque.Context;
using Microsoft.EntityFrameworkCore;

namespace APILojaEstoque.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly APILojaEstoqueContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(APILojaEstoqueContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<T>> FindByNameAsync(string name)
        {
            return await _dbSet
              .Where(e => EF.Property<string>(e, "Nome").Contains(name))
              .ToListAsync(); 
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
           
        }
        public Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;

        }
        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;

        }

        public async Task<T?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Nome") == name);
        }
    }
}
