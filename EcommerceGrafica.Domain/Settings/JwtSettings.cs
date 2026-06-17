namespace EcommerceGrafica.Domain.Settings
{
    public record JwtSettings
    {
        public string Issuer { get; set; } = "ecommercegrafica";
        public string Audience { get; set; } = "ecommercegrafica.web";

        // Chave de assinatura HMAC SHA-256 (mínimo 32 chars). Deve vir de variável de ambiente em produção.
        public string SigningKey { get; set; } = string.Empty;

        public int AccessTokenMinutes { get; set; } = 15;
        public int RefreshTokenDays { get; set; } = 7;
    }
}
