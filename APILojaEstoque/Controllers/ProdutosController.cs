using APILojaEstoque.Context;
using APILojaEstoque.DTOs;
using APILojaEstoque.Models;
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
            ILogger<ProdutosController> logger,
            IMapper mapper)
            : base(unitOfWork, logger, mapper)
        {
        }
    }
}
