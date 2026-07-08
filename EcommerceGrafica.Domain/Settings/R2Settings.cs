namespace EcommerceGrafica.Domain.Settings
{
    public record R2Settings
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccessKeyId { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;

        // URL pública do bucket (ex.: "https://pub-<hash>.r2.dev"). Concatenada com o
        // object key para formar a URL final salva no banco.
        public string PublicBaseUrl { get; set; } = string.Empty;
    }
}
