using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class ProdutoImagemRepository(DbConnection connection) : IProdutoImagemRepository
    {
        private readonly DbConnection _connection = connection;

        public async Task<IReadOnlyList<ProdutoImagemModel>> ListarPorProduto(int produtoId)
        {
            var sql = @"SELECT  id          AS Id,
                                produto_id  AS ProdutoId,
                                url         AS Url,
                                ordem       AS Ordem
                        FROM    public.produto_imagens
                        WHERE   produto_id = @ProdutoId
                        ORDER BY ordem, id";

            var parameters = new DynamicParameters();
            parameters.Add("@ProdutoId", produtoId, DbType.Int32);

            try
            {
                var rows = await _connection.Connection.QueryAsync<ProdutoImagemModel>(sql, parameters);
                return rows.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListarPorProduto Imagens - Erro: " + ex.Message);
                return Array.Empty<ProdutoImagemModel>();
            }
        }

        public async Task RegistrarImagens(int produtoId, IReadOnlyList<ProdutoImagemModel> imagens)
        {
            if (imagens.Count == 0)
                return;

            var sql = @"INSERT INTO public.produto_imagens (produto_id, url, ordem)
                        VALUES (@ProdutoId, @Url, @Ordem)";

            var parameters = imagens.Select(img => new
            {
                ProdutoId = produtoId,
                Url = img.Url,
                Ordem = img.Ordem
            });

            try
            {
                await _connection.Connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RegistrarImagens - Erro: " + ex.Message);
                throw new DomainException(
                    "Não foi possível salvar as imagens do produto no banco. " +
                    "Verifique se a tabela produto_imagens existe (redeploy da API executa Schema.sql).");
            }
        }
    }
}
