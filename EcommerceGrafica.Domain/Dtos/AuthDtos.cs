using EcommerceGrafica.Domain.Enums;

namespace EcommerceGrafica.Domain.Dtos
{
    public sealed record RegisterRequest(
        string Nome,
        string Email,
        string Senha,
        string? Telefone,
        bool ConsentimentoLgpd,
        string? ConsentimentoVersao = null);

    public sealed record LoginRequest(string Email, string Senha);

    public sealed record UsuarioDto(
        int Id,
        string Email,
        string Role,
        int? ClienteId,
        string? Nome,
        DateTime CriadoEm,
        DateTime? UltimoLoginEm,
        DateTime? ConsentimentoEm)
    {
        public static UsuarioDto FromModel(Model.UsuarioModel usuario, string? nome) => new(
            usuario.Id,
            usuario.Email,
            usuario.Role.ToString().ToLowerInvariant(),
            usuario.ClienteId,
            nome,
            usuario.CriadoEm,
            usuario.UltimoLoginEm,
            usuario.ConsentimentoEm);
    }

    public sealed record AuthResponse(UsuarioDto Usuario);
}
