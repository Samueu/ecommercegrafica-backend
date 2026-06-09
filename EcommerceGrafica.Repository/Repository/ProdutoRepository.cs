using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class ProdutoRepository(DbConnection connection) : IProdutoRepository
    {
        private readonly DbConnection _connection = connection;

        public async Task<IEnumerable<ProdutoModel>> ListarAtivos()
        {
            var sql = @"SELECT  id          AS Id,
                                nome        AS Nome,
                                descricao   AS Descricao,
                                preco       AS Preco,
                                moeda       AS Moeda,
                                tipo        AS Tipo,
                                ativo       AS Ativo,
                                criado_em   AS CriadoEm
                        FROM    public.produtos
                        WHERE   ativo = TRUE
                        ORDER BY nome";

            try
            {
                return await _connection.Connection.QueryAsync<ProdutoModel>(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListarAtivos - Erro ao consultar produtos: " + ex.Message);
                return Enumerable.Empty<ProdutoModel>();
            }
        }

        public async Task<ProdutoModel?> GetById(Guid id)
        {
            var sql = @"SELECT  id          AS Id,
                                nome        AS Nome,
                                descricao   AS Descricao,
                                preco       AS Preco,
                                moeda       AS Moeda,
                                tipo        AS Tipo,
                                ativo       AS Ativo,
                                criado_em   AS CriadoEm
                        FROM    public.produtos
                        WHERE   id = @Id";

            DynamicParameters parameters = new();
            parameters.Add("@Id", id, DbType.Guid);

            try
            {
                return await _connection.Connection.QueryFirstOrDefaultAsync<ProdutoModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetById Produto - Erro ao consultar produto: " + ex.Message);
                return null;
            }
        }

        public async Task RegisterProduto(ProdutoModel produto)
        {
            var sql = @"INSERT INTO public.produtos
                            (id, nome, descricao, preco, moeda, tipo, ativo, criado_em)
                        VALUES
                            (@Id, @Nome, @Descricao, @Preco, @Moeda, @Tipo, @Ativo, @CriadoEm)";

            DynamicParameters parameters = new();
            parameters.Add("@Id", produto.Id, DbType.Guid);
            parameters.Add("@Nome", produto.Nome, DbType.String);
            parameters.Add("@Descricao", produto.Descricao, DbType.String);
            parameters.Add("@Preco", produto.Preco, DbType.Decimal);
            parameters.Add("@Moeda", produto.Moeda, DbType.String);
            parameters.Add("@Tipo", (int)produto.Tipo, DbType.Int32);
            parameters.Add("@Ativo", produto.Ativo, DbType.Boolean);
            parameters.Add("@CriadoEm", produto.CriadoEm, DbType.DateTime);

            try
            {
                await _connection.Connection.ExecuteAsync(sql, parameters);
                Console.WriteLine("RegisterProduto - Produto inserido com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("RegisterProduto - Erro ao inserir produto: " + ex.Message);
                throw;
            }
        }
    }
}
