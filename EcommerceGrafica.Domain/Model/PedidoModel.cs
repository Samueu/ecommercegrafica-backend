using EcommerceGrafica.Domain.Enums;

namespace EcommerceGrafica.Domain.Model
{
    public class PedidoModel
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public StatusPedido Status { get; set; }
        public DateTime CriadoEm { get; set; }

        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }

        public List<ItemPedidoModel> Itens { get; set; } = new();
    }
}
