namespace APILojaEstoque.DTOs
{
    public class ProdutoReadDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;
    }
}
