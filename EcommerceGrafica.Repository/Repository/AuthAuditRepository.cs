using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class AuthAuditRepository(DbConnection connection) : IAuthAuditRepository
    {
        private readonly DbConnection _connection = connection;

        public async Task RegisterEvento(AuthAuditModel evento)
        {
            var sql = @"INSERT INTO public.auth_audit
                            (usuario_id, email_tentativa, evento, sucesso, ip_origem, user_agent, detalhe, criado_em)
                        VALUES
                            (@UsuarioId, @EmailTentativa, @Evento, @Sucesso, @IpOrigem, @UserAgent, @Detalhe, @CriadoEm)";

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", (object?)evento.UsuarioId ?? DBNull.Value, DbType.Int32);
            parameters.Add("@EmailTentativa", evento.EmailTentativa ?? string.Empty, DbType.String);
            parameters.Add("@Evento", evento.Evento, DbType.String);
            parameters.Add("@Sucesso", evento.Sucesso, DbType.Boolean);
            parameters.Add("@IpOrigem", (object?)evento.IpOrigem ?? DBNull.Value, DbType.String);
            parameters.Add("@UserAgent", (object?)evento.UserAgent ?? DBNull.Value, DbType.String);
            parameters.Add("@Detalhe", (object?)evento.Detalhe ?? DBNull.Value, DbType.String);
            parameters.Add("@CriadoEm", evento.CriadoEm, DbType.DateTime);

            try
            {
                await _connection.Connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                // Auditoria não deve quebrar o fluxo principal: log apenas.
                Console.WriteLine("AuthAudit - Erro ao gravar evento: " + ex.Message);
            }
        }
    }
}
