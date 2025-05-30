using TesteBluData.Models;
using Microsoft.EntityFrameworkCore;

namespace TesteBluData.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Telefone> Telefones => Set<Telefone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Empresa>(eb =>
        {
            eb.HasKey(e => e.Id);
            eb.Property(e => e.UF).IsRequired();
            eb.Property(e => e.NomeFantasia).IsRequired();
            eb.Property(e => e.CNPJ).IsRequired();
        });

        modelBuilder.Entity<Fornecedor>(fb =>
        {
            fb.HasKey(f => f.Id);
            fb.Property(f => f.Nome).IsRequired();
            fb.Property(f => f.CPFouCNPJ).IsRequired();
            fb.Property(f => f.DataCadastro).IsRequired();
            fb.HasOne(f => f.Empresa).WithMany(e => e.Fornecedores).HasForeignKey(f => f.EmpresaId);
        });

        modelBuilder.Entity<Telefone>(tb =>
        {
            tb.HasKey(t => t.Id);
            tb.Property(t => t.Numero).IsRequired();
            tb.HasOne(t => t.Fornecedor).WithMany(f => f.Telefones).HasForeignKey(t => t.FornecedorId);
        });
    }
}