namespace Wallet.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; } // Permitir set para o Guid.NewGuid()
        public Guid UsuarioId { get; set; } // Adicione esta linha se não existir
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propriedade de navegação (opcional, mas recomendada)
        public virtual Usuario? Usuario { get; set; }

        // Construtor vazio para o EF Core
        public Wallet() { }

        // Construtor para facilitar a criação no Service
        public Wallet(Guid usuarioId)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }
    }
}