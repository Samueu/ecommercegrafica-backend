using EcommerceGrafica.Domain.Enums;

namespace EcommerceGrafica.Domain.Model
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool Ativo { get; set; } = true;
        public DateTime CriadoEm { get; set; }
        public DateTime? UltimoLoginEm { get; set; }

        // Vinculo opcional 1:1 com cliente (cadastro do storefront).
        // Mantém usuários "puros" (ex.: admin interno) sem ter de existir como cliente.
        public int? ClienteId { get; set; }

        // LGPD: registro de consentimento expresso do titular dos dados.
        public DateTime? ConsentimentoEm { get; set; }
        public string? ConsentimentoVersao { get; set; }
    }
}
