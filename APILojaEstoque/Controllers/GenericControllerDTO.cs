using APILojaEstoque.DTOs;
using APILojaEstoque.Interfaces;
using APILojaEstoque.Models;
using APILojaEstoque.Pagination;
using APILojaEstoque.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericControllerDTO<TEntity, TReadDto, TCreateDto, TUpdateDto> : ControllerBase
        where TEntity : class, IEntidade where TUpdateDto : class
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;

        public GenericControllerDTO(
        IUnitOfWork unitOfWork,
        ILogger logger,
        IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

            _repository = typeof(TEntity).Name switch
            {
                nameof(Produtos) => (IGenericRepository<TEntity>)unitOfWork.Produtos,
                nameof(Estoque) => (IGenericRepository<TEntity>)unitOfWork.Estoque,
                _ => throw new ArgumentException($"Tipo '{typeof(TEntity).Name}' não suportado.")
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TReadDto>>> GetAllAsync()
        {
            _logger.LogInformation("GET → Buscando todos os " +
                "registros de {Entity}", typeof(TEntity).Name);

            var entities = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TReadDto>>(entities);

            _logger.LogInformation("GET → {Count} registros " +
                "encontrados para {Entity}", dtos.Count(), typeof(TEntity).Name);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TReadDto>> GetByIdAsync(int id)
        {
            _logger.LogInformation("GET → Buscando {Entity} com ID " +
                "= {Id}", typeof(TEntity).Name, id);

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("GET → {Entity} com ID = {Id} " +
                    "não encontrado", typeof(TEntity).Name, id);
                return NotFound();
            }

            return Ok(_mapper.Map<TReadDto>(entity));
        }

        [HttpGet("paginado")]
        public async Task<ActionResult<PagedResult<TReadDto>>> GetPagedAsync(
        [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET → Buscando {Entity} paginados: página {Page}, tamanho {Size}",
                typeof(TEntity).Name, pageNumber, pageSize);

            if (_repository.GetAll == null)
                return BadRequest("Repositório não suporta consulta IQueryable.");

            var query = _repository.GetAll();

            var pagedResult = await PaginationHelper.CreateAsync<TEntity, TReadDto>(
                query, pageNumber, pageSize, _mapper);

            return Ok(pagedResult);
        }

        [HttpGet("Busca-Por-Nome")]

        public async Task<ActionResult<PagedResult<TReadDto>>> GetByNomeAsync([FromQuery] FiltroNome filtroNome)
        {
            _logger.LogInformation("GET → Buscando {Entity} filtrados por nome: {Nome}", typeof(TEntity).Name, 
                filtroNome.Nome);

            if (!(_repository is IGenericRepository<TEntity> genericRepo))
                return BadRequest("Repositório não suporta busca por nome.");

            var pagedResult = await genericRepo.GetFiltroNomeAsync(filtroNome);

            var mappedResult = new PagedResult<TReadDto>
            {
                Items = _mapper.Map<IEnumerable<TReadDto>>(pagedResult.Items),
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalItems = pagedResult.TotalItems
            };

            return Ok(mappedResult);
        }

        [HttpPost]
        public async Task<ActionResult<TReadDto>> PostAsync([FromBody] TCreateDto dto)
        {
            _logger.LogInformation("POST → Criando novo {Entity}", typeof(TEntity).Name);

            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("POST → {Entity} criado com " +
                "sucesso (ID = {Id})", typeof(TEntity).Name, entity.Id);
            return Ok(_mapper.Map<TReadDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TReadDto>> PutAsync(int id, [FromBody] TUpdateDto dto)
        {
            _logger.LogInformation("PUT → Atualizando {Entity} com " +
                "ID = {Id}", typeof(TEntity).Name, id);

            var entity = _mapper.Map<TEntity>(dto);
            entity.Id = id;

            await _repository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("PUT → {Entity} atualizado com " +
                "sucesso (ID = {Id})", typeof(TEntity).Name, id);
            return Ok(_mapper.Map<TReadDto>(entity));
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody] JsonPatchDocument<TUpdateDto> patchDoc) 
        {
            _logger.LogInformation("PATCH → Atualizando parcialmente {Entity} com ID = {Id}",
                typeof(TEntity).Name, id);

            if (patchDoc == null)
                return BadRequest("Documento de patch não pode ser nulo.");

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("PATCH → {Entity} com ID = {Id} não encontrado", typeof(TEntity).Name, id);
                return NotFound();
            }

            var dto = _mapper.Map<TUpdateDto>(entity);

            patchDoc.ApplyTo(dto, ModelState);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PATCH → Dados inválidos para atualização de {Entity} com ID = {Id}", 
                    typeof(TEntity).Name, id);
                return BadRequest(ModelState);
            }

            TryValidateModel(dto);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PATCH → Validação falhou após aplicar patch para {Entity} com ID = {Id}",
                    typeof(TEntity).Name, id);
                return BadRequest(ModelState);
            }

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("PATCH → {Entity} atualizado com sucesso (ID = {Id})", 
                typeof(TEntity).Name, id);

            return Ok(_mapper.Map<TReadDto>(entity));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation("DELETE → Buscando {Entity} com ID = " +
                "{Id} para exclusão", typeof(TEntity).Name, id);

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("DELETE → {Entity} com ID = {Id} " +
                    "não encontrado", typeof(TEntity).Name, id);
                return NotFound();
            }

            await _repository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("DELETE → {Entity} com ID = {Id} " +
                "removido com sucesso", typeof(TEntity).Name, id);
            return NoContent();
        }
    }
}
