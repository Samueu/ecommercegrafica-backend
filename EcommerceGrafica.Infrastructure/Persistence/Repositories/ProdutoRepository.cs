using EcommerceGrafica.Domain.Catalog.Entities;
using EcommerceGrafica.Domain.Catalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGrafica.Infrastructure.Persistence.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Produto>> ListarAtivosAsync(CancellationToken cancellationToken = default) =>
        await _context.Produtos
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Produto aggregate, CancellationToken cancellationToken = default) =>
        await _context.Produtos.AddAsync(aggregate, cancellationToken);

    public void Update(Produto aggregate) => _context.Produtos.Update(aggregate);

    public void Remove(Produto aggregate) => _context.Produtos.Remove(aggregate);
}
