using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class ClienteRepository(DbConnection connection) : IClienteRepository
    {
        private readonly DbConnection _connection = connection;

        public async Task<IEnumerable<ClienteModel>> ListarTodos()
        {
            var sql = @"SELECT  id              AS Id,
                                nome            AS Nome,
                                email           AS Email,
                                telefone        AS Telefone,
                                cadastrado_em   AS CadastradoEm
                        FROM    public.clientes
                        ORDER BY nome";

            try
            {
                return await _connection.Connection.QueryAsync<ClienteModel>(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListarTodos Cliente - Erro ao consultar clientes: " + ex.Message);
                return Enumerable.Empty<ClienteModel>();
            }
        }

        public async Task<ClienteModel?> GetById(int id)
        {
            var sql = @"SELECT  id              AS Id,
                                nome            AS Nome,
                                email           AS Email,
                                telefone        AS Telefone,
                                cadastrado_em   AS CadastradoEm
                        FROM    public.clientes
                        WHERE   id = @Id";

            DynamicParameters parameters = new();
            parameters.Add("@Id", id, DbType.Int32);

            try
            {
                return await _connection.Connection.QueryFirstOrDefaultAsync<ClienteModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetById Cliente - Erro ao consultar cliente: " + ex.Message);
                return null;
            }
        }

        public async Task<ClienteModel?> GetByEmail(string email)
        {
            var sql = @"SELECT  id              AS Id,
                                nome            AS Nome,
                                email           AS Email,
                                telefone        AS Telefone,
                                cadastrado_em   AS CadastradoEm
                        FROM    public.clientes
                        WHERE   email = @Email";

            DynamicParameters parameters = new();
            parameters.Add("@Email", email, DbType.String);

            try
            {
                return await _connection.Connection.QueryFirstOrDefaultAsync<ClienteModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetByEmail Cliente - Erro ao consultar cliente: " + ex.Message);
                return null;
            }
        }

        public async Task RegisterCliente(ClienteModel cliente)
        {
            var sql = @"INSERT INTO public.clientes
                            (nome, email, telefone, cadastrado_em)
                        VALUES
                            (@Nome, @Email, @Telefone, @CadastradoEm)
                        RETURNING id";

            DynamicParameters parameters = new();
            parameters.Add("@Nome", cliente.Nome, DbType.String);
            parameters.Add("@Email", cliente.Email, DbType.String);
            parameters.Add("@Telefone", (object?)cliente.Telefone ?? DBNull.Value, DbType.String);
            parameters.Add("@CadastradoEm", cliente.CadastradoEm, DbType.DateTime);

            try
            {
                cliente.Id = await _connection.Connection.ExecuteScalarAsync<int>(sql, parameters);
                Console.WriteLine("RegisterCliente - Cliente inserido com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("RegisterCliente - Erro ao inserir cliente: " + ex.Message);
                throw;
            }
        }
    }
}
