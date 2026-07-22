using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceGrafica.Application.Security
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;
        private readonly SigningCredentials _signingCredentials;

        public JwtTokenGenerator(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;

            if (string.IsNullOrWhiteSpace(_settings.SigningKey) || _settings.SigningKey.Length < 32)
                throw new DomainException("JwtSettings.SigningKey deve ter ao menos 32 caracteres.");

            var keyBytes = Encoding.UTF8.GetBytes(_settings.SigningKey);
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);
        }

        public AccessToken CreateAccessToken(UsuarioModel usuario)
        {
            var agora = DateTime.UtcNow;
            var expira = agora.AddMinutes(_settings.AccessTokenMinutes);

            var role = usuario.Role.ToString().ToLowerInvariant();

            // Claims mínimos (princípio da minimização de dados — LGPD art. 6º, III).
            // IMPORTANTE: usar o claim curto "role" — Program.cs define RoleClaimType = "role".
            // ClaimTypes.Role serializa com URI longa e [Authorize(Roles = "admin")] falha (403).
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new("role", role)
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: agora,
                expires: expira,
                signingCredentials: _signingCredentials);

            var encoded = new JwtSecurityTokenHandler().WriteToken(token);
            return new AccessToken(encoded, expira);
        }

        public RefreshTokenIssued CreateRefreshToken()
        {
            // 64 bytes de entropia → base64url ~86 chars. Suficiente contra colisão/força bruta.
            Span<byte> buffer = stackalloc byte[64];
            RandomNumberGenerator.Fill(buffer);
            var raw = Base64UrlEncoder.Encode(buffer.ToArray());
            var hash = HashRefreshToken(raw);
            var expira = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);
            return new RefreshTokenIssued(raw, hash, expira);
        }

        public string HashRefreshToken(string token)
        {
            // SHA-256 é o suficiente porque o token bruto já é aleatório e de alta entropia;
            // o hash existe para que um vazamento do banco não revele os tokens em uso.
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
