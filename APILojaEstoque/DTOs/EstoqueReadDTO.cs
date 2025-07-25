namespace APILojaEstoque.DTOs
{
    public class EstoqueReadDTO
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string Local { get; set; } = "Matriz";
        public int Quantidade { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
