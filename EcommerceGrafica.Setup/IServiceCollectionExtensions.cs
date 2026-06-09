using EcommerceGrafica.Application.Service;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Repository.Data;
using EcommerceGrafica.Repository.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceGrafica.Setup
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection IocConfiguration(this IServiceCollection services, IConfiguration configuration) => services
            .RegisterInternalService()
            .RegisterInternalRepository()
            .RegisterConnection();

        private static IServiceCollection RegisterInternalService(this IServiceCollection services) => services
            .AddScoped<IProdutoService, ProdutoService>()
            .AddScoped<IClienteService, ClienteService>()
            .AddScoped<IPedidoService, PedidoService>();

        private static IServiceCollection RegisterInternalRepository(this IServiceCollection services) => services
            .AddScoped<IProdutoRepository, ProdutoRepository>()
            .AddScoped<IClienteRepository, ClienteRepository>()
            .AddScoped<IPedidoRepository, PedidoRepository>();

        private static IServiceCollection RegisterConnection(this IServiceCollection services) => services
            .AddScoped<DbConnection>();
    }
}
