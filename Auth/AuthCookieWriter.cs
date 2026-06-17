using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Settings;
using Microsoft.Extensions.Options;

namespace ecommercegrafica.Auth
{
    /// <summary>
    /// Centraliza a escrita/limpeza dos cookies httpOnly de access e refresh token.
    /// Mantém a configuração (SameSite, Secure, Domain) num único lugar e fora dos controllers.
    /// </summary>
    public sealed class AuthCookieWriter
    {
        private readonly AuthCookieSettings _settings;

        public AuthCookieWriter(IOptions<AuthCookieSettings> options)
        {
            _settings = options.Value;
        }

        public string AccessTokenName => _settings.AccessTokenName;
        public string RefreshTokenName => _settings.RefreshTokenName;

        public void WriteTokens(HttpResponse response, AccessToken access, RefreshTokenIssued refresh)
        {
            response.Cookies.Append(
                _settings.AccessTokenName,
                access.Token,
                BuildOptions(access.ExpiraEm, path: "/"));

            // O refresh token vive apenas no endpoint que dele precisa,
            // reduzindo a superfície em que ele é enviado pelo browser.
            response.Cookies.Append(
                _settings.RefreshTokenName,
                refresh.Token,
                BuildOptions(refresh.ExpiraEm, path: "/api/auth"));
        }

        public void Clear(HttpResponse response)
        {
            // Para deletar é preciso reproduzir Path/Domain/SameSite/Secure exatos.
            var expirado = DateTimeOffset.UnixEpoch;

            response.Cookies.Append(
                _settings.AccessTokenName, string.Empty, BuildOptions(expirado.UtcDateTime, path: "/"));

            response.Cookies.Append(
                _settings.RefreshTokenName, string.Empty, BuildOptions(expirado.UtcDateTime, path: "/api/auth"));
        }

        private CookieOptions BuildOptions(DateTime expiraEmUtc, string path) => new()
        {
            HttpOnly = true,
            Secure = _settings.Secure,
            SameSite = ParseSameSite(_settings.SameSite),
            Expires = new DateTimeOffset(expiraEmUtc, TimeSpan.Zero),
            Path = path,
            Domain = string.IsNullOrWhiteSpace(_settings.Domain) ? null : _settings.Domain,
            IsEssential = true
        };

        private static SameSiteMode ParseSameSite(string value) => value?.Trim().ToLowerInvariant() switch
        {
            "none" => SameSiteMode.None,
            "strict" => SameSiteMode.Strict,
            _ => SameSiteMode.Lax,
        };
    }
}
