using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoModel>> ListarTodos();
        Task<PedidoModel?> ObterPorId(Guid id);
        Task<IEnumerable<PedidoModel>> ListarPorCliente(Guid clienteId);
        Task<PedidoModel> RegistrarPedido(PedidoModel pedido);
    }
}
