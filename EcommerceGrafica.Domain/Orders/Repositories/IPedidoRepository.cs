using EcommerceGrafica.Domain.Orders.Entities;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Orders.Repositories;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<IReadOnlyList<Pedido>> ListarPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
