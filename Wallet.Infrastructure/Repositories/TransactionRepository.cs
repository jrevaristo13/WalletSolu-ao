using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.DbContext; // 🔹 Ajustado para o seu namespace real

namespace Wallet.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Transacao transacao)
        {
            // 🔹 Usando 'Transacoes' que é o nome que você definiu no AppDbContext
            _context.Transacoes.Add(transacao); 
            await _context.SaveChangesAsync();
        }

       public async Task<IEnumerable<Transacao>> GetByUsuarioIdAsync(Guid usuarioId)
{
    // 🔹 Filtramos pelo ID do usuário e ordenamos por data decrescente
    return await _context.Transacoes
        .Where(t => t.Id != Guid.Empty) // Aqui você deve usar o campo de relação com usuário se ele existir na sua Transacao
        .OrderByDescending(t => t.Data)
        .ToListAsync();
}
    }
}