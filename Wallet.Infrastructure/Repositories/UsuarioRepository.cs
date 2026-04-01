using Dapper;  // ← Para consultas com Dapper (se estiver usando)
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.DbContext;
using System.Data;

namespace Wallet.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        private readonly IDbConnection _dbConnection;  // ← Injetado via DI

        public UsuarioRepository(AppDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        public async Task<Usuario> GetByIdAsync(Guid id)
        {
            // Opção A: Usando EF Core (recomendado)
            return await _context.Usuarios.FindAsync(id);
            
            // Opção B: Usando Dapper (se preferir)
            // return await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(
            //     "SELECT * FROM Usuarios WHERE Id = @Id", new { Id = id });
        }

        public async Task<Usuario> GetByUsernameAsync(string username)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task DeleteAsync(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}