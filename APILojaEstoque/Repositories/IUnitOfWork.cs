using APILojaEstoque.Models;

namespace APILojaEstoque.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Produtos> Produtos { get; }
        IGenericRepository<Estoque> Estoque { get; }
        Task<int> CommitAsync();
    }   
    
}
