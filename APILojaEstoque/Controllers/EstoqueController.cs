using APILojaEstoque.Context;
using APILojaEstoque.DTOs;
using APILojaEstoque.Interfaces;
using APILojaEstoque.Models;
using APILojaEstoque.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstoqueController : GenericControllerDTO<
    Estoque,
    EstoqueReadDTO,
    EstoqueCreateDTO,
    EstoqueUpdateDTO>
    {
        public EstoqueController(
            IUnitOfWork unitOfWork,
            ILogger<EstoqueController> logger,
            IMapper mapper)
            : base(unitOfWork, logger, mapper)
        {
        }
    }
}
