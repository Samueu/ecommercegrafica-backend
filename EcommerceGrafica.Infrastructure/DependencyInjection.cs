using EcommerceGrafica.Application.Common.Interfaces;
using EcommerceGrafica.Domain.Catalog.Repositories;
using EcommerceGrafica.Domain.Customers.Repositories;
using EcommerceGrafica.Domain.Orders.Repositories;
using EcommerceGrafica.Infrastructure.Persistence;
using EcommerceGrafica.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceGrafica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("EcommerceGraficaDb"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}
