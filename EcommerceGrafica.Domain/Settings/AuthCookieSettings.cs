namespace EcommerceGrafica.Domain.Settings
{
    /// <summary>
    /// Configura como os cookies de autenticação são emitidos pelo backend.
    /// Em dev (localhost) costuma-se usar SameSite=Lax + Secure=false.
    /// Em produção, frontend e API em domínios distintos exigem SameSite=None + Secure=true.
    /// </summary>
    public record AuthCookieSettings
    {
        public string AccessTokenName { get; set; } = "eg_at";
        public string RefreshTokenName { get; set; } = "eg_rt";

        /// <summary>Lax | None | Strict. Cross-site requer "None" + Secure=true.</summary>
        public string SameSite { get; set; } = "Lax";

        public bool Secure { get; set; } = false;

        /// <summary>Domínio do cookie. Vazio = host atual. Útil quando front e API compartilham eTLD+1.</summary>
        public string? Domain { get; set; }
    }
}
