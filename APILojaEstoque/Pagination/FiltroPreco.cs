namespace APILojaEstoque.Pagination
{
    public class FiltroPreco : QueryStringParameters
    {
        public decimal? Preco { get; set; }
        public string? Ordenacao { get; set; } // "Maior", "Menor", "Igual"
    }
}
