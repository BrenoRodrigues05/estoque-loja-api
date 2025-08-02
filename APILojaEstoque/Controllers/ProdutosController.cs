using APILojaEstoque.Context;
using APILojaEstoque.DTOs;
using APILojaEstoque.Models;
using APILojaEstoque.Pagination;
using APILojaEstoque.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : GenericControllerDTO<
    Produtos,
    ProdutoReadDTO,
    ProdutoCreateDTO,
    ProdutoUpdateDTO>
    {
        public ProdutosController(
    IUnitOfWork unitOfWork,
    ILogger<GenericControllerDTO<Produtos, ProdutoReadDTO, ProdutoCreateDTO, ProdutoUpdateDTO>> logger,
    IMapper mapper)
    : base(unitOfWork, logger, mapper)
        {
        }

        [HttpGet("filtro-preco")]
        public async Task<ActionResult<PagedResult<ProdutoReadDTO>>> GetByPrecoAsync([FromQuery] FiltroPreco filtro)
        {
            _logger.LogInformation("GET → Filtrando {Entity} por preço: {Filtro}", typeof(Produtos).Name, filtro);

            var hasPrecoProp = typeof(Produtos).GetProperty("Preco") != null;
            if (!hasPrecoProp)
                return BadRequest($"A entidade {typeof(Produtos).Name} não possui a propriedade 'Preco'.");

            var result = await _repository.GetFiltrosPrecoAsync(filtro);

            var mapped = new PagedResult<ProdutoReadDTO>
            {
                Items = _mapper.Map<IEnumerable<ProdutoReadDTO>>(result.Items),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems
            };

            return Ok(mapped);
        }
    }
    
}
