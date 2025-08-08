using APILojaEstoque.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace APILojaEstoque.Context
{
    public class APILojaEstoqueContext : IdentityDbContext <ApplicationUser>
    {
        public APILojaEstoqueContext(DbContextOptions<APILojaEstoqueContext> options) : base(options) { }

        public DbSet<Estoque>? Estoques { get; set; }

        public DbSet<Produtos>? Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Produtos>()
                .HasMany(p => p.Estoques)
                .WithOne(e => e.Produto)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }

       
    }
}
