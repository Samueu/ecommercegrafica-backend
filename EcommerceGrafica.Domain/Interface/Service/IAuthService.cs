using EcommerceGrafica.Domain.Dtos;
using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    /// <summary>
    /// Resultado de operações que emitem credenciais: contém o objeto do usuário
    /// e os tokens (access + refresh) já prontos para serem gravados em cookies.
    /// </summary>
    public sealed record AuthResult(
        UsuarioDto Usuario,
        AccessToken AccessToken,
        RefreshTokenIssued RefreshToken);

    public interface IAuthService
    {
        Task<AuthResult> Register(RegisterRequest request, RequestContext ctx);
        Task<AuthResult> Login(LoginRequest request, RequestContext ctx);
        Task<AuthResult> Refresh(string refreshTokenRaw, RequestContext ctx);
        Task Logout(string? refreshTokenRaw, RequestContext ctx);
        Task<UsuarioDto?> ObterUsuarioPorId(int id);
        Task ExcluirConta(int usuarioId, RequestContext ctx);
    }

    /// <summary>
    /// Dados do contexto da requisição usados para auditoria.
    /// </summary>
    public sealed record RequestContext(string? IpOrigem, string? UserAgent);
}
