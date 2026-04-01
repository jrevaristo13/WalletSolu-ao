using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet.Domain.Entities.Wallet> CreateAsync(Wallet.Domain.Entities.Wallet wallet);
        Task<Wallet.Domain.Entities.Wallet> GetByUsuarioIdAsync(Guid usuarioId);
        Task UpdateAsync(Wallet.Domain.Entities.Wallet wallet);
    }
}
