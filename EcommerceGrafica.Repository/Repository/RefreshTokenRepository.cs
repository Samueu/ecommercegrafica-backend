using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class RefreshTokenRepository(DbConnection connection) : IRefreshTokenRepository
    {
        private readonly DbConnection _connection = connection;

        public async Task<long> RegisterToken(RefreshTokenModel token)
        {
            var sql = @"INSERT INTO public.refresh_tokens
                            (usuario_id, token_hash, criado_em, expira_em, ip_origem, user_agent)
                        VALUES
                            (@UsuarioId, @TokenHash, @CriadoEm, @ExpiraEm, @IpOrigem, @UserAgent)
                        RETURNING id";

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", token.UsuarioId, DbType.Int32);
            parameters.Add("@TokenHash", token.TokenHash, DbType.String);
            parameters.Add("@CriadoEm", token.CriadoEm, DbType.DateTime);
            parameters.Add("@ExpiraEm", token.ExpiraEm, DbType.DateTime);
            parameters.Add("@IpOrigem", (object?)token.IpOrigem ?? DBNull.Value, DbType.String);
            parameters.Add("@UserAgent", (object?)token.UserAgent ?? DBNull.Value, DbType.String);

            token.Id = await _connection.Connection.ExecuteScalarAsync<long>(sql, parameters);
            return token.Id;
        }

        public async Task<RefreshTokenModel?> GetByHash(string tokenHash)
        {
            var sql = @"SELECT  id                 AS Id,
                                usuario_id         AS UsuarioId,
                                token_hash         AS TokenHash,
                                criado_em          AS CriadoEm,
                                expira_em          AS ExpiraEm,
                                revogado_em        AS RevogadoEm,
                                motivo_revogacao   AS MotivoRevogacao,
                                ip_origem          AS IpOrigem,
                                user_agent         AS UserAgent,
                                substituido_por_id AS SubstituidoPorId
                        FROM    public.refresh_tokens
                        WHERE   token_hash = @TokenHash";

            var parameters = new DynamicParameters();
            parameters.Add("@TokenHash", tokenHash, DbType.String);

            return await _connection.Connection.QueryFirstOrDefaultAsync<RefreshTokenModel>(sql, parameters);
        }

        public async Task RevogarToken(long id, string motivo, DateTime quandoUtc, long? substituidoPorId = null)
        {
            var sql = @"UPDATE public.refresh_tokens
                        SET    revogado_em = @Quando,
                               motivo_revogacao = @Motivo,
                               substituido_por_id = @SubstituidoPorId
                        WHERE  id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);
            parameters.Add("@Quando", quandoUtc, DbType.DateTime);
            parameters.Add("@Motivo", motivo, DbType.String);
            parameters.Add("@SubstituidoPorId", (object?)substituidoPorId ?? DBNull.Value, DbType.Int64);

            await _connection.Connection.ExecuteAsync(sql, parameters);
        }

        public async Task RevogarTodosDoUsuario(int usuarioId, string motivo, DateTime quandoUtc)
        {
            var sql = @"UPDATE public.refresh_tokens
                        SET    revogado_em = @Quando,
                               motivo_revogacao = @Motivo
                        WHERE  usuario_id = @UsuarioId
                          AND  revogado_em IS NULL";

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", usuarioId, DbType.Int32);
            parameters.Add("@Quando", quandoUtc, DbType.DateTime);
            parameters.Add("@Motivo", motivo, DbType.String);

            await _connection.Connection.ExecuteAsync(sql, parameters);
        }
    }
}
