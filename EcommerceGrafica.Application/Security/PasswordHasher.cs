using EcommerceGrafica.Domain.Interface.Service;

namespace EcommerceGrafica.Application.Security
{
    /// <summary>
    /// Implementação baseada em BCrypt (algoritmo adaptativo, salt embutido no hash).
    /// Atende ao princípio de segurança da LGPD (art. 46): senhas nunca trafegam nem
    /// repousam em claro no banco.
    /// </summary>
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;

        public string Hash(string senhaEmClaro)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(senhaEmClaro);
            return BCrypt.Net.BCrypt.EnhancedHashPassword(senhaEmClaro, WorkFactor);
        }

        public bool Verify(string senhaEmClaro, string hashArmazenado)
        {
            if (string.IsNullOrWhiteSpace(senhaEmClaro) || string.IsNullOrWhiteSpace(hashArmazenado))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.EnhancedVerify(senhaEmClaro, hashArmazenado);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }

        public bool NeedsRehash(string hashArmazenado)
        {
            if (string.IsNullOrWhiteSpace(hashArmazenado))
                return true;

            try
            {
                return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashArmazenado, WorkFactor);
            }
            catch
            {
                return true;
            }
        }
    }
}
