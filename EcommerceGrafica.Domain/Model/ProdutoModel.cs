using EcommerceGrafica.Domain.Enums;

namespace EcommerceGrafica.Domain.Model
{
    public class ProdutoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Moeda { get; set; } = "BRL";
        public TipoProduto Tipo { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
