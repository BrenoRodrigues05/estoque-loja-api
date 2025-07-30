using APILojaEstoque.Models;
using APILojaEstoque.Pagination;
using APILojaEstoque.Repositories;
using AutoMapper;

namespace APILojaEstoque.Services
{
    public class LojaService
    {
       private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LojaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<TReadDto>> GetPagedProductsAsync<TReadDto>(
            int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Produtos.GetAll().AsQueryable();
            return await PaginationHelper.CreateAsync<Produtos, TReadDto>(query, pageNumber, pageSize, _mapper);
        }
    }
}
