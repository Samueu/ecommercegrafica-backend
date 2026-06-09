using System.Data;
using Dapper;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Model;
using EcommerceGrafica.Repository.Data;

namespace EcommerceGrafica.Repository.Repository
{
    public class PedidoRepository(DbConnection connection) : IPedidoRepository
    {
        private readonly DbConnection _connection = connection;

        public async Task<IEnumerable<PedidoModel>> ListarTodos()
        {
            return await CarregarPedidosAsync(@"SELECT * FROM public.pedidos ORDER BY criado_em DESC");
        }

        public async Task<PedidoModel?> GetById(Guid id)
        {
            var pedidoSql = @"SELECT    id          AS Id,
                                        cliente_id  AS ClienteId,
                                        status      AS Status,
                                        criado_em   AS CriadoEm,
                                        logradouro  AS Logradouro,
                                        numero      AS Numero,
                                        bairro      AS Bairro,
                                        cidade      AS Cidade,
                                        estado      AS Estado,
                                        cep         AS Cep
                              FROM      public.pedidos
                              WHERE     id = @Id";

            var itensSql = @"SELECT     id              AS Id,
                                        pedido_id       AS PedidoId,
                                        produto_id      AS ProdutoId,
                                        nome_produto    AS NomeProduto,
                                        quantidade      AS Quantidade,
                                        preco_unitario  AS PrecoUnitario
                             FROM       public.itens_pedido
                             WHERE      pedido_id = @PedidoId";

            DynamicParameters pedidoParams = new();
            pedidoParams.Add("@Id", id, DbType.Guid);

            try
            {
                var pedido = await _connection.Connection.QueryFirstOrDefaultAsync<PedidoModel>(pedidoSql, pedidoParams);
                if (pedido is null)
                    return null;

                DynamicParameters itensParams = new();
                itensParams.Add("@PedidoId", pedido.Id, DbType.Guid);

                var itens = await _connection.Connection.QueryAsync<ItemPedidoModel>(itensSql, itensParams);
                pedido.Itens = itens.ToList();
                return pedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetById Pedido - Erro ao consultar pedido: " + ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<PedidoModel>> ListarPorCliente(Guid clienteId)
        {
            var sql = @"SELECT  id          AS Id,
                                cliente_id  AS ClienteId,
                                status      AS Status,
                                criado_em   AS CriadoEm,
                                logradouro  AS Logradouro,
                                numero      AS Numero,
                                bairro      AS Bairro,
                                cidade      AS Cidade,
                                estado      AS Estado,
                                cep         AS Cep
                        FROM    public.pedidos
                        WHERE   cliente_id = @ClienteId
                        ORDER BY criado_em DESC";

            DynamicParameters parameters = new();
            parameters.Add("@ClienteId", clienteId, DbType.Guid);

            try
            {
                return await _connection.Connection.QueryAsync<PedidoModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListarPorCliente Pedido - Erro ao consultar pedidos: " + ex.Message);
                return Enumerable.Empty<PedidoModel>();
            }
        }

        public async Task RegisterPedido(PedidoModel pedido)
        {
            var pedidoSql = @"INSERT INTO public.pedidos
                                (id, cliente_id, status, criado_em,
                                 logradouro, numero, bairro, cidade, estado, cep)
                              VALUES
                                (@Id, @ClienteId, @Status, @CriadoEm,
                                 @Logradouro, @Numero, @Bairro, @Cidade, @Estado, @Cep)";

            var itemSql = @"INSERT INTO public.itens_pedido
                                (id, pedido_id, produto_id, nome_produto, quantidade, preco_unitario)
                            VALUES
                                (@Id, @PedidoId, @ProdutoId, @NomeProduto, @Quantidade, @PrecoUnitario)";

            await using var transaction = await _connection.Connection.BeginTransactionAsync();

            try
            {
                DynamicParameters pedidoParams = new();
                pedidoParams.Add("@Id", pedido.Id, DbType.Guid);
                pedidoParams.Add("@ClienteId", pedido.ClienteId, DbType.Guid);
                pedidoParams.Add("@Status", (int)pedido.Status, DbType.Int32);
                pedidoParams.Add("@CriadoEm", pedido.CriadoEm, DbType.DateTime);
                pedidoParams.Add("@Logradouro", (object?)pedido.Logradouro ?? DBNull.Value, DbType.String);
                pedidoParams.Add("@Numero", (object?)pedido.Numero ?? DBNull.Value, DbType.String);
                pedidoParams.Add("@Bairro", (object?)pedido.Bairro ?? DBNull.Value, DbType.String);
                pedidoParams.Add("@Cidade", (object?)pedido.Cidade ?? DBNull.Value, DbType.String);
                pedidoParams.Add("@Estado", (object?)pedido.Estado ?? DBNull.Value, DbType.String);
                pedidoParams.Add("@Cep", (object?)pedido.Cep ?? DBNull.Value, DbType.String);

                await _connection.Connection.ExecuteAsync(pedidoSql, pedidoParams, transaction);

                foreach (var item in pedido.Itens)
                {
                    DynamicParameters itemParams = new();
                    itemParams.Add("@Id", item.Id, DbType.Guid);
                    itemParams.Add("@PedidoId", pedido.Id, DbType.Guid);
                    itemParams.Add("@ProdutoId", item.ProdutoId, DbType.Guid);
                    itemParams.Add("@NomeProduto", item.NomeProduto, DbType.String);
                    itemParams.Add("@Quantidade", item.Quantidade, DbType.Int32);
                    itemParams.Add("@PrecoUnitario", item.PrecoUnitario, DbType.Decimal);

                    await _connection.Connection.ExecuteAsync(itemSql, itemParams, transaction);
                }

                await transaction.CommitAsync();
                Console.WriteLine("RegisterPedido - Pedido inserido com sucesso.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("RegisterPedido - Erro ao inserir pedido: " + ex.Message);
                throw;
            }
        }

        private async Task<IEnumerable<PedidoModel>> CarregarPedidosAsync(string sql)
        {
            try
            {
                var pedidos = (await _connection.Connection.QueryAsync<PedidoModel>(sql)).ToList();
                if (pedidos.Count == 0)
                    return pedidos;

                var pedidoIds = pedidos.Select(p => p.Id).ToArray();
                var itens = await _connection.Connection.QueryAsync<ItemPedidoModel>(
                    @"SELECT    id              AS Id,
                                pedido_id       AS PedidoId,
                                produto_id      AS ProdutoId,
                                nome_produto    AS NomeProduto,
                                quantidade      AS Quantidade,
                                preco_unitario  AS PrecoUnitario
                      FROM      public.itens_pedido
                      WHERE     pedido_id = ANY(@PedidoIds)",
                    new { PedidoIds = pedidoIds });

                var itensPorPedido = itens.GroupBy(i => i.PedidoId).ToDictionary(g => g.Key, g => g.ToList());
                foreach (var pedido in pedidos)
                {
                    pedido.Itens = itensPorPedido.TryGetValue(pedido.Id, out var lista) ? lista : new List<ItemPedidoModel>();
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CarregarPedidosAsync - Erro: " + ex.Message);
                return Enumerable.Empty<PedidoModel>();
            }
        }
    }
}
