using APILojaEstoque.Context;
using APILojaEstoque.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : GenericController<Produtos>
    {
        public ProdutosController(APILojaEstoqueContext context, ILogger<GenericController<Produtos>> logger)
            : base(context, logger)
        {
        }
    }
}
