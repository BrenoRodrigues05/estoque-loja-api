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
    public class GenericControllerDTO<TEntity, TReadDto, TCreateDto, TUpdateDto> : ControllerBase
        where TEntity : class, IEntidade
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
