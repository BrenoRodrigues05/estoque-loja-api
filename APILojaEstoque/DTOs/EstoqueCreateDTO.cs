using System.ComponentModel.DataAnnotations;

namespace APILojaEstoque.DTOs
{
    public class EstoqueCreateDTO
    {
        [Required]
        public int ProdutoId { get; set; }

        [Required]
        [StringLength(20)]
        public string Local { get; set; } = "Matriz";

        [Range(0, int.MaxValue)]
        public int Quantidade { get; set; }

        [Required]
        public DateTime UltimaAtualizacao { get; set; }
    }
}
