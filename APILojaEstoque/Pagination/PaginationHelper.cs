using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APILojaEstoque.Pagination
{
    public class PaginationHelper
    {
        public static async Task<PagedResult<TDto>> CreateAsync<TEntity, TDto>(
        IQueryable<TEntity> query, int pageNumber, int pageSize, IMapper mapper)
        {
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoItems = mapper.Map<List<TDto>>(items);

            return new PagedResult<TDto>
            {
                Items = dtoItems,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
