using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EcommerceGrafica.Application.Service
{
    public class R2StorageService : IStorageService
    {
        private static readonly string[] TiposImagemPermitidos =
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        private static readonly string[] ExtensoesPermitidas =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5 MB

        private readonly R2Settings _settings;
        private readonly ILogger<R2StorageService> _logger;

        public R2StorageService(IOptions<R2Settings> options, ILogger<R2StorageService> logger)
        {
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<string> UploadProdutoImagemAsync(IFormFile arquivo, CancellationToken cancellationToken = default)
        {
            ValidarConfiguracao();
            ValidarArquivo(arquivo);

            var extensao = (Path.GetExtension(arquivo.FileName) ?? string.Empty)
                .Trim()
                .ToLowerInvariant();
            if (string.IsNullOrEmpty(extensao))
            {
                extensao = InferirExtensaoPorContentType(arquivo.ContentType);
            }

            var objectKey = $"produtos/{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}{extensao}";

            var credentials = new BasicAWSCredentials(_settings.AccessKeyId, _settings.SecretAccessKey);
            var config = new AmazonS3Config
            {
                // Endpoint S3-compatible do Cloudflare R2.
                ServiceURL = $"https://{_settings.AccountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            };

            using var s3Client = new AmazonS3Client(credentials, config);
            await using var stream = arquivo.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = objectKey,
                InputStream = stream,
                ContentType = string.IsNullOrWhiteSpace(arquivo.ContentType)
                    ? "application/octet-stream"
                    : arquivo.ContentType,
                // Necessário no R2: o SDK novo da AWS envia Streaming SigV4 com trailer
                // de checksum, que o R2 rejeita. Essas duas flags são obrigatórias.
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            try
            {
                await s3Client.PutObjectAsync(request, cancellationToken);
                _logger.LogInformation("Upload de imagem de produto concluido no R2. ObjectKey={ObjectKey}.", objectKey);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar imagem para o R2. ObjectKey={ObjectKey}, StatusCode={StatusCode}.", objectKey, ex.StatusCode);
                throw new DomainException("Não foi possível enviar a imagem para o armazenamento. Tente novamente.");
            }

            return $"{_settings.PublicBaseUrl.TrimEnd('/')}/{objectKey}";
        }

        private void ValidarConfiguracao()
        {
            if (string.IsNullOrWhiteSpace(_settings.AccountId) ||
                string.IsNullOrWhiteSpace(_settings.AccessKeyId) ||
                string.IsNullOrWhiteSpace(_settings.SecretAccessKey) ||
                string.IsNullOrWhiteSpace(_settings.BucketName) ||
                string.IsNullOrWhiteSpace(_settings.PublicBaseUrl))
            {
                throw new InvalidOperationException(
                    "Configuração do Cloudflare R2 incompleta. Verifique a seção 'R2' em appsettings ou as variáveis de ambiente (R2__AccountId, R2__AccessKeyId, R2__SecretAccessKey, R2__BucketName, R2__PublicBaseUrl).");
            }
        }

        private static void ValidarArquivo(IFormFile arquivo)
        {
            if (arquivo is null || arquivo.Length <= 0)
                throw new DomainException("Arquivo de imagem inválido.");

            if (arquivo.Length > TamanhoMaximoBytes)
                throw new DomainException("A imagem excede o tamanho máximo permitido (5 MB).");

            var contentType = (arquivo.ContentType ?? string.Empty).Trim().ToLowerInvariant();
            var extensao = (Path.GetExtension(arquivo.FileName) ?? string.Empty).Trim().ToLowerInvariant();

            var contentTypeValido = TiposImagemPermitidos.Contains(contentType);
            var extensaoValida = ExtensoesPermitidas.Contains(extensao);

            if (!contentTypeValido && !extensaoValida)
                throw new DomainException("Formato de imagem inválido. Envie apenas JPG, PNG ou WEBP.");
        }

        private static string InferirExtensaoPorContentType(string? contentType)
        {
            return (contentType ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".bin"
            };
        }
    }
}
