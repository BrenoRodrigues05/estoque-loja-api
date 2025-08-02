
using APILojaEstoque.Context;
using APILojaEstoque.Pagination;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<PagedResult<T>> GetFiltrosPrecoAsync(FiltroPreco filtroPrecoParams)
        {
            var query = GetAll().AsQueryable();

            if (filtroPrecoParams.Preco.HasValue && !string.IsNullOrEmpty(filtroPrecoParams.Ordenacao))
            {
                var preco = filtroPrecoParams.Preco.Value;

                if (filtroPrecoParams.Ordenacao.Equals("Maior", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => EF.Property<decimal>(p, "Preco") > preco)
                        .OrderBy(p => EF.Property<decimal>(p, "Preco"));
                }
                else if (filtroPrecoParams.Ordenacao.Equals("Menor", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => EF.Property<decimal>(p, "Preco") < preco)
                         .OrderBy(p => EF.Property<decimal>(p, "Preco"));
                }
                else if (filtroPrecoParams.Ordenacao.Equals("Igual", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => EF.Property<decimal>(p, "Preco") == preco)
                         .OrderBy(p => EF.Property<decimal>(p, "Preco"));
                }
            }
            var ProdutosFiltrados = PaginationHelper.ToPagedList(query, filtroPrecoParams.PageNumber,
                filtroPrecoParams.PageSize);

            return ProdutosFiltrados;
        }

        public async Task<PagedResult<T>> GetFiltroNomeAsync(FiltroNome filtroNomeParams)
        {
            var query = GetAll().AsQueryable();

            string? propriedadeParaBuscar = null;

            if (typeof(T).GetProperty("Nome") != null)
                propriedadeParaBuscar = "Nome";
            else if (typeof(T).GetProperty("Local") != null)
                propriedadeParaBuscar = "Local";

            if (!string.IsNullOrEmpty(filtroNomeParams.Nome) && propriedadeParaBuscar != null)
            {
                var list = await query.ToListAsync();

                list = list
                    .Where(e =>
                    {
                        var prop = typeof(T).GetProperty(propriedadeParaBuscar);
                        if (prop == null) return false;

                        var value = prop.GetValue(e) as string;
                        return value != null && value.Contains(filtroNomeParams.Nome, 
                            StringComparison.OrdinalIgnoreCase);
                    })
                    .ToList();

                return PaginationHelper.ToPagedList(list.AsQueryable(), filtroNomeParams.PageNumber, 
                    filtroNomeParams.PageSize);
            }

            var FiltradoPorNome = PaginationHelper.ToPagedList(query, filtroNomeParams.PageNumber, 
                filtroNomeParams.PageSize);
            return FiltradoPorNome; 
        }
    }
}
