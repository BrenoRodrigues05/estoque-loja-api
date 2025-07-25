using System.ComponentModel.DataAnnotations;

namespace APILojaEstoque.DTOs
{
    public class ProdutoCreateDTO
    {
        [Required]
        [StringLength(10)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(50)]
        public string CodigoBarras { get; set; } = string.Empty;
    }
}
