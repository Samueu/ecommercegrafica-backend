using EcommerceGrafica.Domain.Catalog.Entities;
using EcommerceGrafica.Domain.Catalog.Enums;
using EcommerceGrafica.Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceGrafica.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        if (await context.Produtos.AnyAsync())
            return;

        var produtos = new[]
        {
            Produto.Criar("Cartão de Visita 500un", "Cartão couché 300g", new Preco(89.90m), TipoProduto.CartaoVisita),
            Produto.Criar("Banner 2x1m", "Banner em lona com acabamento", new Preco(149.90m), TipoProduto.Banner),
            Produto.Criar("Folder A4", "Folder frente e verso", new Preco(199.90m), TipoProduto.Folder)
        };

        await context.Produtos.AddRangeAsync(produtos);
        await context.SaveChangesAsync();
    }
}
