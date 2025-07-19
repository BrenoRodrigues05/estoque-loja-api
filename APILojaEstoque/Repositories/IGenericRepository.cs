namespace APILojaEstoque.Repositories
{
    public interface IGenericRepository <T>  where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindByNameAsync(string name);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> GetByNameAsync(string name);
       
    }
}
