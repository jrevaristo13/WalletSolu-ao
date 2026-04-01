using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task CreateAsync(Transacao transacao);
        Task<IEnumerable<Transacao>> GetByUsuarioIdAsync(Guid usuarioId);
    }
}
