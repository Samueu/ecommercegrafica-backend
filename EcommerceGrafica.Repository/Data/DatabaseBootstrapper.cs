using EcommerceGrafica.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EcommerceGrafica.Repository.Data
{
    public static class DatabaseBootstrapper
    {
        public static async Task EnsureSchemaAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<ConnectionStrings>>();

            var connectionString = options.Value.Postgres;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("DatabaseBootstrapper - ConnectionStrings.Postgres não configurado. Pulando.");
                return;
            }

            var scriptsDirectory = Path.Combine(AppContext.BaseDirectory, "Scripts");
            var schemaPath = Path.Combine(scriptsDirectory, "Schema.sql");
            var seedPath = Path.Combine(scriptsDirectory, "Seed.sql");

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                if (File.Exists(schemaPath))
                {
                    var schemaSql = await File.ReadAllTextAsync(schemaPath);
                    await using var schemaCmd = new NpgsqlCommand(schemaSql, connection);
                    await schemaCmd.ExecuteNonQueryAsync();
                    Console.WriteLine("DatabaseBootstrapper - Schema.sql executado com sucesso.");
                }

                if (File.Exists(seedPath))
                {
                    var seedSql = await File.ReadAllTextAsync(seedPath);
                    await using var seedCmd = new NpgsqlCommand(seedSql, connection);
                    await seedCmd.ExecuteNonQueryAsync();
                    Console.WriteLine("DatabaseBootstrapper - Seed.sql executado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseBootstrapper - Erro ao preparar o banco: " + ex.Message);
                throw;
            }
        }
    }
}
