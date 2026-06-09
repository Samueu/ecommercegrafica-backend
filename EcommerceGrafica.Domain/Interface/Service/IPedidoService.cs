using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoModel>> ListarTodos();
        Task<PedidoModel?> ObterPorId(int id);
        Task<IEnumerable<PedidoModel>> ListarPorCliente(int clienteId);
        Task<PedidoModel> RegistrarPedido(PedidoModel pedido);
    }
}
