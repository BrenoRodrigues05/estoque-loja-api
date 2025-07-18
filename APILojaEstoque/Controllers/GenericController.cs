using APILojaEstoque.Context;
using APILojaEstoque.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace APILojaEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class, IEntidade
    {
        private readonly APILojaEstoqueContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<GenericController<T>> _logger;

        public GenericController(APILojaEstoqueContext context, ILogger<GenericController<T>> logger)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _logger = logger;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
        {
            _logger.LogInformation("GET → Buscando todos os registros de {Entity}", typeof(T).Name);
            var entities = await _dbSet.AsNoTracking().ToListAsync();

            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetByIdAsync(int idGet)
        {
            _logger.LogInformation("GET → Buscando {Entity} com ID = {Id}", typeof(T).Name, idGet);
            var entity = await _dbSet.FindAsync(idGet);
            if (entity == null) {
                _logger.LogWarning("GET → {Entity} com ID = {Id} não encontrado", typeof(T).Name, idGet);
                return NotFound($"Id:'{idGet}' Não encontrado no sistema");
            }
            return Ok(entity);
        }

        [HttpGet("Busca-Por-Nome")]

        public async Task<ActionResult<IEnumerable<T>>> GetByNameAsync([FromQuery] string nameGet)
        {
            _logger.LogInformation("GET → Buscando {entities} com nome = {nameGet}", typeof(T).Name, nameGet);
            var entities = await _dbSet
                .AsNoTracking()
                .Where(e => EF.Property<string>(e, "Nome").Contains(nameGet))
                .ToListAsync();

            if (entities == null || entities.Count == 0) {
                _logger.LogWarning("GET → {Entity} com nome = {nameGet} não encontrado", typeof(T).Name, nameGet);
                return NotFound($"Nenhum resultado encontrado com o nome '{nameGet}'.");
            }

            return Ok(entities);
        }

        [HttpPost]

        public async Task<ActionResult<T>> PostAsync([FromBody] T entity)
        {
            await _dbSet.AddAsync(entity);

            try
            {
                await _context.SaveChangesAsync();

                var id = typeof(T).GetProperty("Id")?.GetValue(entity);
                _logger.LogInformation("POST → Novo {Entity} criado com ID = {Id}", typeof(T).Name, id);

                return Created($"{Request.Path}/{id}", entity);
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? "Sem mensagem interna";

                return StatusCode(500, new
                {
                    Message = "Erro ao salvar no banco.",
                    ExceptionMessage = dbEx.Message,
                    InnerExceptionMessage = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPut("{IdPut:int}")]

        public async Task<ActionResult<T>> PutAsync(int IdPut, [FromBody] T entity)
        {
            if (IdPut != entity.Id)
            {
                _logger.LogWarning("PUT → Tentativa de atualizar {Entity} com ID inconsistente (IdPut = " +
                    "{IdPut}, entity.Id = {EntityId})", typeof(T).Name, IdPut, entity.Id);
                return BadRequest($"Id:'{IdPut}' Não encontrado no sistema");
            }
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("PUT → {Entity} com ID = {IdPut} atualizado", typeof(T).Name, IdPut);
            return Ok(entity);
        }

        [HttpPut("Editar-Por-Nome")]
        public async Task<ActionResult<T>> PutByName([FromQuery] string name, [FromBody] T entity)
        {
            var entityBanco = await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Nome") == name);

            if (entityBanco == null) {
                _logger.LogWarning("PUT → {Entity} com nome = '{Name}' não encontrado", typeof(T).Name, name);
                return NotFound($"Nome '{name}' não encontrado no sistema..."); }

            entity.Id = entityBanco.Id;

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            _logger.LogInformation("PUT → {Entity} com nome = '{Name}' atualizado com sucesso " +
                "(ID = {Id})", typeof(T).Name, name, entity.Id);
            return Ok(entity);
        }

        [HttpDelete("{idDelete:int}")]

        public async Task <IActionResult> DeleteAsync (int idDelete)
        {
            var entity = await _dbSet.FindAsync(idDelete);

            if (entity == null)
            {
                _logger.LogWarning("DELETE → {Entity} com ID = " +
                    "{IdDelete} não encontrado", typeof(T).Name, idDelete);
                return NotFound($"Id:'{idDelete}' Não foi encontrado no sistema...");
            }
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("DELETE → {Entity} com ID = {IdDelete} removido", typeof(T).Name, idDelete);
            return NoContent();
        }

        [HttpDelete("Deletar-Por-Nome")]

        public async Task <IActionResult> DeleteAsync([FromQuery] string name)
        {
            _logger.LogInformation("DELETE → Buscando {Entity} com nome = " +
                "'{Name}' para exclusão", typeof(T).Name, name);
            var entity = await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Nome") == name);

            if (entity == null)
            {
                _logger.LogWarning("DELETE → {Entity} com nome = '{Name}' não encontrado", typeof(T).Name, name);
                return NotFound($"Nome '{name}' não foi encontrado no sistema.");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("DELETE → {Entity} com nome = '{Name}' " +
                "removido com sucesso (ID = {Id})", typeof(T).Name, name, entity.Id);

            return Ok($"Entidade com nome '{name}' deletada com sucesso.");
        }
    
    }
}
