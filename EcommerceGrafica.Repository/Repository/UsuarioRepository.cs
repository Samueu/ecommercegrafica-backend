using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Enums;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class UsuarioRepository(DbConnection connection) : IUsuarioRepository
    {
        private readonly DbConnection _connection = connection;

        private const string BaseSelect = @"
            SELECT  id                    AS Id,
                    email                 AS Email,
                    senha_hash            AS SenhaHash,
                    role                  AS Role,
                    ativo                 AS Ativo,
                    criado_em             AS CriadoEm,
                    ultimo_login_em       AS UltimoLoginEm,
                    cliente_id            AS ClienteId,
                    consentimento_em      AS ConsentimentoEm,
                    consentimento_versao  AS ConsentimentoVersao
            FROM    public.usuarios";

        public async Task<UsuarioModel?> GetById(int id)
        {
            var sql = BaseSelect + " WHERE id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            try
            {
                return await _connection.Connection.QueryFirstOrDefaultAsync<UsuarioModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetById Usuario - Erro: " + ex.Message);
                return null;
            }
        }

        public async Task<UsuarioModel?> GetByEmail(string email)
        {
            var sql = BaseSelect + " WHERE email = @Email";
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email, DbType.String);

            try
            {
                return await _connection.Connection.QueryFirstOrDefaultAsync<UsuarioModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetByEmail Usuario - Erro: " + ex.Message);
                return null;
            }
        }

        public async Task RegisterUsuario(UsuarioModel usuario)
        {
            var sql = @"INSERT INTO public.usuarios
                            (email, senha_hash, role, ativo, criado_em, cliente_id,
                             consentimento_em, consentimento_versao)
                        VALUES
                            (@Email, @SenhaHash, @Role, @Ativo, @CriadoEm, @ClienteId,
                             @ConsentimentoEm, @ConsentimentoVersao)
                        RETURNING id";

            var parameters = new DynamicParameters();
            parameters.Add("@Email", usuario.Email, DbType.String);
            parameters.Add("@SenhaHash", usuario.SenhaHash, DbType.String);
            parameters.Add("@Role", (int)usuario.Role, DbType.Int32);
            parameters.Add("@Ativo", usuario.Ativo, DbType.Boolean);
            parameters.Add("@CriadoEm", usuario.CriadoEm, DbType.DateTime);
            parameters.Add("@ClienteId", (object?)usuario.ClienteId ?? DBNull.Value, DbType.Int32);
            parameters.Add("@ConsentimentoEm", (object?)usuario.ConsentimentoEm ?? DBNull.Value, DbType.DateTime);
            parameters.Add("@ConsentimentoVersao", (object?)usuario.ConsentimentoVersao ?? DBNull.Value, DbType.String);

            usuario.Id = await _connection.Connection.ExecuteScalarAsync<int>(sql, parameters);
        }

        public async Task AtualizarUltimoLogin(int id, DateTime quandoUtc)
        {
            var sql = "UPDATE public.usuarios SET ultimo_login_em = @Quando WHERE id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("@Quando", quandoUtc, DbType.DateTime);
            parameters.Add("@Id", id, DbType.Int32);

            await _connection.Connection.ExecuteAsync(sql, parameters);
        }

        public async Task DesativarUsuario(int id)
        {
            // Soft-delete: pseudonimiza o e-mail e zera dados sensíveis, mas mantém o id
            // para preservar a integridade referencial dos pedidos passados (obrigação legal/fiscal).
            var sql = @"UPDATE public.usuarios
                        SET    email = CONCAT('anon_', id, '@removed.local'),
                               senha_hash = '',
                               ativo = FALSE,
                               consentimento_em = NULL,
                               consentimento_versao = NULL
                        WHERE  id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            await _connection.Connection.ExecuteAsync(sql, parameters);
        }
    }
}
