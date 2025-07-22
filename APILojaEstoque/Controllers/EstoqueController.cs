using APILojaEstoque.Context;
using APILojaEstoque.Interfaces;
using APILojaEstoque.Models;
using APILojaEstoque.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstoqueController : GenericController<Estoque>
    {
        public EstoqueController(IUnitOfWork unitOfWork, ILogger<GenericController<Estoque>> logger)
     : base(unitOfWork, logger)
        {
        }


    }
}
