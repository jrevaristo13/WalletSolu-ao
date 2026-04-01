using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.DbContext
{
    /// <summary>
    /// Contexto principal de dados da aplicação Wallet.
    /// Responsável pelo mapeamento Objeto-Relacional (ORM) via Entity Framework Core.
    /// </summary>
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        /// <summary>
        /// Construtor padrão sem parâmetros.
        /// Necessário para ferramentas de design-time do EF Core (migrations).
        /// ⚠️ Não use este construtor em runtime — utilize sempre a injeção de dependência.
        /// </summary>
        public AppDbContext() { }

        /// <summary>
        /// Construtor principal injetado pelo container de DI em runtime.
        /// </summary>
        /// <param name="options">Configurações do contexto de banco de dados.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        // 🔹 DbSets - Representam as tabelas do banco de dados
        public DbSet<Usuario> Usuarios { get; set; }
        
        /// <summary>
        /// DbSet para a entidade Wallet.
        /// Observação: Usa namespace completo para evitar conflito com o namespace do DbContext.
        /// </summary>
        public DbSet<Wallet.Domain.Entities.Wallet> Wallets { get; set; }
        
        public DbSet<Transacao> Transacoes { get; set; }

        /// <summary>
        /// Configura o modelo de dados via Fluent API.
        /// Executado uma vez durante a inicialização do contexto.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo do EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Configuração da entidade Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 🔹 Configuração da entidade Wallet
            modelBuilder.Entity<Wallet.Domain.Entities.Wallet>(entity =>
            {
                entity.ToTable("Wallets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 🔹 Configuração da entidade Transacao
            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.ToTable("Transactions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Valor).HasPrecision(18, 2);
                entity.Property(e => e.Data).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Conversão do enum Tipo para string no banco de dados
                entity.Property(e => e.Tipo).HasConversion<string>();
            });
        }

        /// <summary>
        /// Método opcional para configuração adicional antes da inicialização.
        /// Pode ser usado para logs ou configurações específicas de provedor.
        /// </summary>
        /// <param name="optionsBuilder">Construtor de opções do DbContext.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ⚠️ Este método só é executado se o contexto for instanciado SEM opções injetadas
            // Para design-time, prefira usar IDesignTimeDbContextFactory em vez de configurar aqui
            if (!optionsBuilder.IsConfigured)
            {
                // Exemplo (não recomendado para produção):
                // optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WalletDb;Trusted_Connection=True;");
            }
        }
    }
}