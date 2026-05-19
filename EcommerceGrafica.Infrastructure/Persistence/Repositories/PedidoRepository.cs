using EcommerceGrafica.Domain.Orders.Entities;
using EcommerceGrafica.Domain.Orders.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGrafica.Infrastructure.Persistence.Repositories;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Pedido>> ListarPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await _context.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Pedido aggregate, CancellationToken cancellationToken = default) =>
        await _context.Pedidos.AddAsync(aggregate, cancellationToken);

    public void Update(Pedido aggregate) => _context.Pedidos.Update(aggregate);

    public void Remove(Pedido aggregate) => _context.Pedidos.Remove(aggregate);
}
