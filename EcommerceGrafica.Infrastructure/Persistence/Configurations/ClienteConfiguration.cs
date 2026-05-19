using EcommerceGrafica.Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceGrafica.Infrastructure.Persistence.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.CadastradoEm)
            .IsRequired();

        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(x => x.Valor)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            email.HasIndex(x => x.Valor).IsUnique();
        });

        builder.Ignore(c => c.DomainEvents);
    }
}
