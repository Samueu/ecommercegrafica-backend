using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<PedidoModel>> ListarTodos();
        Task<PedidoModel?> GetById(Guid id);
        Task<IEnumerable<PedidoModel>> ListarPorCliente(Guid clienteId);
        Task RegisterPedido(PedidoModel pedido);
    }
}
