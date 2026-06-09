using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<PedidoModel>> ListarTodos();
        Task<PedidoModel?> GetById(int id);
        Task<IEnumerable<PedidoModel>> ListarPorCliente(int clienteId);
        Task RegisterPedido(PedidoModel pedido);
    }
}
