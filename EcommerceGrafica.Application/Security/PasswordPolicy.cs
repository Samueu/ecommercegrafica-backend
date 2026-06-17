using System.Text.RegularExpressions;
using EcommerceGrafica.Domain.Exceptions;

namespace EcommerceGrafica.Application.Security
{
    /// <summary>
    /// Política de senha forte aplicada no servidor (defesa em profundidade).
    /// Mínimo: 8 caracteres, ao menos uma letra maiúscula, uma minúscula, um dígito e um símbolo.
    /// </summary>
    public static partial class PasswordPolicy
    {
        public const int TamanhoMinimo = 8;
        public const int TamanhoMaximo = 128;

        public static void Validar(string? senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new DomainException("A senha é obrigatória.");

            if (senha.Length < TamanhoMinimo)
                throw new DomainException($"A senha deve ter pelo menos {TamanhoMinimo} caracteres.");

            if (senha.Length > TamanhoMaximo)
                throw new DomainException($"A senha não pode ter mais de {TamanhoMaximo} caracteres.");

            if (!HasUpperRegex().IsMatch(senha))
                throw new DomainException("A senha deve conter pelo menos uma letra maiúscula.");

            if (!HasLowerRegex().IsMatch(senha))
                throw new DomainException("A senha deve conter pelo menos uma letra minúscula.");

            if (!HasDigitRegex().IsMatch(senha))
                throw new DomainException("A senha deve conter pelo menos um número.");

            if (!HasSymbolRegex().IsMatch(senha))
                throw new DomainException("A senha deve conter pelo menos um símbolo (!@#$%&*…).");
        }

        [GeneratedRegex(@"[A-Z]")]
        private static partial Regex HasUpperRegex();

        [GeneratedRegex(@"[a-z]")]
        private static partial Regex HasLowerRegex();

        [GeneratedRegex(@"\d")]
        private static partial Regex HasDigitRegex();

        [GeneratedRegex(@"[^A-Za-z0-9]")]
        private static partial Regex HasSymbolRegex();
    }
}
