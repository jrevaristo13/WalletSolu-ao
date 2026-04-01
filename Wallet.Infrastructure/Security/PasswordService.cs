using Microsoft.AspNetCore.Identity;
using Wallet.Application.Interfaces;

namespace Wallet.Infrastructure.Security
{
    public class SenhaService : ISenhaService
    {
        private readonly PasswordHasher<string> _passwordHasher;

        public SenhaService()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        public string HashPassword(string password)
        {
            // ✅ Usa null! para evitar warning CS8625
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}