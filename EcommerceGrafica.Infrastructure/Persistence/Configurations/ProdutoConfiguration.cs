using EcommerceGrafica.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceGrafica.Infrastructure.Persistence.Configurations;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Tipo)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired();

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.OwnsOne(p => p.Preco, preco =>
        {
            preco.Property(x => x.Valor)
                .HasColumnName("Preco")
                .HasPrecision(18, 2)
                .IsRequired();

            preco.Property(x => x.Moeda)
                .HasColumnName("Moeda")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Ignore(p => p.DomainEvents);
    }
}
