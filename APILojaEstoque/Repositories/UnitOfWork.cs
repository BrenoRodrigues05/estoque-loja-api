using APILojaEstoque.Context;
using APILojaEstoque.Models;

namespace APILojaEstoque.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
         public IGenericRepository<Produtos> Produtos { get; }
         public IGenericRepository<Estoque> Estoque { get; }

        private readonly APILojaEstoqueContext _context;

        public UnitOfWork(APILojaEstoqueContext context)
        {
            _context = context;

            Produtos = new GenericRepository<Produtos>(_context);
            Estoque = new GenericRepository<Estoque>(_context);
        }
      
        public async Task<int> CommitAsync()
        {
           return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
           _context.Dispose();
        }
    }
}
