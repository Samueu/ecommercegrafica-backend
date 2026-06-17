namespace EcommerceGrafica.Domain.Model
{
    public class RefreshTokenModel
    {
        public long Id { get; set; }
        public int UsuarioId { get; set; }

        // Nunca armazenamos o token em claro: guardamos o hash (SHA-256).
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; }
        public DateTime ExpiraEm { get; set; }
        public DateTime? RevogadoEm { get; set; }
        public string? MotivoRevogacao { get; set; }

        // Metadados úteis para auditoria e detecção de uso indevido.
        public string? IpOrigem { get; set; }
        public string? UserAgent { get; set; }

        // Rotação: quando este token é trocado por outro, guardamos o id do sucessor.
        public long? SubstituidoPorId { get; set; }

        public bool EstaAtivo(DateTime agoraUtc) =>
            RevogadoEm is null && ExpiraEm > agoraUtc;
    }
}
