using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public sealed record AccessToken(string Token, DateTime ExpiraEm);
    public sealed record RefreshTokenIssued(string Token, string TokenHash, DateTime ExpiraEm);

    public interface IJwtTokenGenerator
    {
        AccessToken CreateAccessToken(UsuarioModel usuario);
        RefreshTokenIssued CreateRefreshToken();
        string HashRefreshToken(string token);
    }
}
