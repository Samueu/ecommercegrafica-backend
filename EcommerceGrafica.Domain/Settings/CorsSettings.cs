namespace EcommerceGrafica.Domain.Settings
{
    public record CorsSettings
    {
        // Lista de origens autorizadas. Cookies cross-site exigem origens específicas (não pode ser '*').
        public string[] AllowedOrigins { get; set; } = System.Array.Empty<string>();
    }
}
