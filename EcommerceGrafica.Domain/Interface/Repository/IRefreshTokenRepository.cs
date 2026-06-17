using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<long> RegisterToken(RefreshTokenModel token);
        Task<RefreshTokenModel?> GetByHash(string tokenHash);
        Task RevogarToken(long id, string motivo, DateTime quandoUtc, long? substituidoPorId = null);
        Task RevogarTodosDoUsuario(int usuarioId, string motivo, DateTime quandoUtc);
    }
}
