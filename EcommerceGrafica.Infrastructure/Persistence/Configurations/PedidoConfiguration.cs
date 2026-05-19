using EcommerceGrafica.Domain.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceGrafica.Infrastructure.Persistence.Configurations;

public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ClienteId)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.OwnsOne(p => p.EnderecoEntrega, endereco =>
        {
            endereco.Property(x => x.Logradouro).HasColumnName("Logradouro").HasMaxLength(200);
            endereco.Property(x => x.Numero).HasColumnName("Numero").HasMaxLength(20);
            endereco.Property(x => x.Bairro).HasColumnName("Bairro").HasMaxLength(100);
            endereco.Property(x => x.Cidade).HasColumnName("Cidade").HasMaxLength(100);
            endereco.Property(x => x.Estado).HasColumnName("Estado").HasMaxLength(2);
            endereco.Property(x => x.Cep).HasColumnName("Cep").HasMaxLength(10);
        });

        builder.OwnsMany(p => p.Itens, itens =>
        {
            itens.ToTable("ItensPedido");
            itens.WithOwner().HasForeignKey("PedidoId");
            itens.HasKey(i => i.Id);
            itens.Property(i => i.ProdutoId).IsRequired();
            itens.Property(i => i.NomeProduto).HasMaxLength(200).IsRequired();
            itens.Property(i => i.Quantidade).IsRequired();
            itens.Property(i => i.PrecoUnitario).HasPrecision(18, 2).IsRequired();
            itens.Ignore(i => i.DomainEvents);
        });

        builder.Navigation(p => p.Itens).Metadata.SetField("_itens");
        builder.Ignore(p => p.DomainEvents);
    }
}
