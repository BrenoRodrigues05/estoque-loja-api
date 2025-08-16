using APILojaEstoque.Context;
using APILojaEstoque.DTOs;
using APILojaEstoque.Interfaces;
using APILojaEstoque.Models;
using APILojaEstoque.Repositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
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
