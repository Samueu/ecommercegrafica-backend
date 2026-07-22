namespace EcommerceGrafica.Domain.Model
{
    public class ProdutoImagemModel
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Ordem { get; set; }
    }
}
