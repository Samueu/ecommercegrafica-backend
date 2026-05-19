using EcommerceGrafica.Domain.Customers.Entities;
using EcommerceGrafica.Domain.Customers.Repositories;
using EcommerceGrafica.Domain.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGrafica.Infrastructure.Persistence.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Cliente?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await _context.Clientes.FirstOrDefaultAsync(c => c.Email.Valor == email.Valor, cancellationToken);

    public async Task AddAsync(Cliente aggregate, CancellationToken cancellationToken = default) =>
        await _context.Clientes.AddAsync(aggregate, cancellationToken);

    public void Update(Cliente aggregate) => _context.Clientes.Update(aggregate);

    public void Remove(Cliente aggregate) => _context.Clientes.Remove(aggregate);
}
