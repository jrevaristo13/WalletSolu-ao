using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Infrastructure.DbContext;

namespace Wallet.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet.Domain.Entities.Wallet> CreateAsync(Wallet.Domain.Entities.Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet.Domain.Entities.Wallet> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UsuarioId == usuarioId);
        }

        public async Task UpdateAsync(Wallet.Domain.Entities.Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}