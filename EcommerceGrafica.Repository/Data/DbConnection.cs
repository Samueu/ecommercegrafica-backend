using EcommerceGrafica.Domain.Settings;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EcommerceGrafica.Repository.Data
{
    public sealed class DbConnection : IDisposable
    {
        public NpgsqlConnection Connection { get; }
        public ConnectionStrings ConnectionStrings { get; set; }

        public DbConnection(IOptions<ConnectionStrings> connectionStrings)
        {
            ConnectionStrings = connectionStrings.Value;
            Connection = new NpgsqlConnection(ConnectionStrings.Postgres);
            Connection.Open();
        }

        public void Dispose() => Connection.Dispose();
    }
}
