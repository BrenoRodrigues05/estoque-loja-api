using APILojaEstoque.Context;
using APILojaEstoque.Interfaces;
using APILojaEstoque.Models;
using APILojaEstoque.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;


namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class, IEntidade
    {
        private readonly IGenericRepository<T> _repository;
        private readonly ILogger<GenericController<T>> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GenericController(IUnitOfWork unitOfWork, ILogger<GenericController<T>> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

            _repository = typeof(T).Name switch
            {
                nameof(Produtos) => (IGenericRepository<T>)unitOfWork.Produtos,
                nameof(Estoque) => (IGenericRepository<T>)unitOfWork.Estoque,
                _ => throw new ArgumentException($"Tipo de entidade '{typeof(T).Name}' não suportado.")
            };
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("Busca-Por-Nome")]

        public async Task<ActionResult<IEnumerable<T>>> GetByNameAsync([FromQuery] string name)
        {
            var entities = await _repository.FindByNameAsync(name);
            return entities.Any() ? Ok(entities) : NotFound();
        }

        [HttpPost]

        public async Task<ActionResult<T>> PostAsync([FromBody] T entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return Ok(entity);
        }

        [HttpPut("{id:int}")]

        public async Task<ActionResult<T>> PutAsync(int id, [FromBody] T entity)
        {
            if (id != entity.Id) return BadRequest();
            await _repository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("PUT → {Entity} atualizado com sucesso (ID = {Id})", typeof(T).Name, id);
            return Ok(entity);
        }

        [HttpPut("Editar-Por-Nome")]
        public async Task<ActionResult<T>> PutByName([FromQuery] string name, [FromBody] T entity)
        {
            var entityBanco = await _repository.GetByNameAsync(name);

            if (entityBanco == null)
            {
                _logger.LogWarning("PUT → {Entity} com nome = '{Name}' não encontrado", typeof(T).Name, name);
                return NotFound($"Nome '{name}' não encontrado no sistema...");
            }

            entity.Id = entityBanco.Id;

            await _repository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("PUT → {Entity} com nome = '{Name}' atualizado com sucesso (ID = {Id})",
                typeof(T).Name, name, entity.Id);

            return Ok(entity);
        }

        [HttpDelete("{id:int}")]

        public async Task <IActionResult> DeleteAsync (int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _repository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("DELETE → {Entity} removido com sucesso (ID = {Id})", typeof(T).Name, id);

            return NoContent();
        }

        [HttpDelete("Deletar-Por-Nome")]

        public async Task <IActionResult> DeleteAsync([FromQuery] string name)
        {
            _logger.LogInformation("DELETE → Buscando {Entity} com nome = '{Name}' para exclusão", typeof(T).Name, name);

            var entity = await _repository.GetByNameAsync(name);
            if (entity == null)
            {
                _logger.LogWarning("DELETE → {Entity} com nome = '{Name}' não encontrado", typeof(T).Name, name);
                return NotFound($"Nome '{name}' não foi encontrado no sistema.");
            }

            await _repository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("DELETE → {Entity} com nome = '{Name}' removido com sucesso (ID = {Id})",
                typeof(T).Name, name, entity.Id);

            return Ok($"Entidade com nome '{name}' deletada com sucesso.");
        }
    
    }
}
