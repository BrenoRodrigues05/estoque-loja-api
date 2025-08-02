using APILojaEstoque.Interfaces;
using APILojaEstoque.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APILojaEstoque.Models
{
    [Table("Produtos")]
    public class Produtos : IEntidade, ITemPreco
    {
        [Key]
        
        
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(10, ErrorMessage = "O nome deve ter no máximo 10 caracteres.")]
        [PrimeiraLetraMaiúscula]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O código de barras é obrigatório.")]
        [StringLength(50, ErrorMessage = "O código de barras deve ter no máximo 50 caracteres.")]
        public string CodigoBarras { get; set; } = string.Empty;
        
        [JsonIgnore]
        public List<Estoque> Estoques { get; set; } = new();
    }
}
