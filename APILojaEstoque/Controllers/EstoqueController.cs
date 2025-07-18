using APILojaEstoque.Context;
using APILojaEstoque.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstoqueController : GenericController<Estoque>
    {
        public EstoqueController(APILojaEstoqueContext context, ILogger<GenericController<Estoque>> logger)
     : base(context, logger)
        {
        }


    }
}
