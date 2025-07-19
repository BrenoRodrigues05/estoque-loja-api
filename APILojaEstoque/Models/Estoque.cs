using APILojaEstoque.Interfaces;
using APILojaEstoque.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APILojaEstoque.Models
{
    [Table("Estoque")]
    public class Estoque : IEntidade
    {
        [Key]
        
        [JsonIgnore]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ProdutoId é obrigatório.")]
        public int ProdutoId { get; set; }
        [JsonIgnore]
        public Produtos? Produto { get; set; }

        [Required(ErrorMessage = "O local é obrigatório.")]
        [StringLength(20, ErrorMessage = "O local deve ter no máximo 20 caracteres.")]
        [PrimeiraLetraMaiúscula]
        public string Local { get; set; } = "Matriz";

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "A data da última atualização é obrigatória.")]
        public DateTime UltimaAtualizacao { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string Nome => Produto != null ? Produto.Nome : "Estoque sem produto";
    }
}
