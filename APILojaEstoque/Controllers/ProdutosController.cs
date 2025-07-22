using APILojaEstoque.Context;
using APILojaEstoque.Models;
using APILojaEstoque.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : GenericController<Produtos>
    {
        public ProdutosController(IUnitOfWork unitOfWork, ILogger<GenericController<Produtos>> logger)
            : base(unitOfWork, logger)
        {
        }
    }
}
