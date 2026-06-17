namespace EcommerceGrafica.Domain.Model
{
    /// <summary>
    /// Registro de eventos de autenticação para auditoria (apoio à LGPD art. 46/47:
    /// medidas de segurança e rastreabilidade).
    /// </summary>
    public class AuthAuditModel
    {
        public long Id { get; set; }
        public int? UsuarioId { get; set; }
        public string EmailTentativa { get; set; } = string.Empty;
        public string Evento { get; set; } = string.Empty;
        public bool Sucesso { get; set; }
        public string? IpOrigem { get; set; }
        public string? UserAgent { get; set; }
        public string? Detalhe { get; set; }
        public DateTime CriadoEm { get; set; }
    }

    public static class AuthEvent
    {
        public const string Login = "login";
        public const string LoginFalha = "login_falha";
        public const string Logout = "logout";
        public const string Refresh = "refresh";
        public const string RefreshFalha = "refresh_falha";
        public const string Cadastro = "cadastro";
        public const string ExclusaoConta = "exclusao_conta";
    }
}
