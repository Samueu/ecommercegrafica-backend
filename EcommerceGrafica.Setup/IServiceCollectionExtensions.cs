using EcommerceGrafica.Application.Security;
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
            .RegisterSecurity()
            .RegisterConnection();

        private static IServiceCollection RegisterInternalService(this IServiceCollection services) => services
            .AddScoped<IProdutoService, ProdutoService>()
            .AddScoped<IClienteService, ClienteService>()
            .AddScoped<IPedidoService, PedidoService>()
            .AddScoped<IAuthService, AuthService>()
            .AddSingleton<IStorageService, R2StorageService>();

        private static IServiceCollection RegisterInternalRepository(this IServiceCollection services) => services
            .AddScoped<IProdutoRepository, ProdutoRepository>()
            .AddScoped<IProdutoImagemRepository, ProdutoImagemRepository>()
            .AddScoped<IClienteRepository, ClienteRepository>()
            .AddScoped<IPedidoRepository, PedidoRepository>()
            .AddScoped<IUsuarioRepository, UsuarioRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<IAuthAuditRepository, AuthAuditRepository>();

        private static IServiceCollection RegisterSecurity(this IServiceCollection services) => services
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        private static IServiceCollection RegisterConnection(this IServiceCollection services) => services
            .AddScoped<DbConnection>();
    }
}
