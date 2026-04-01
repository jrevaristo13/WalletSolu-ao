using System.Security.Cryptography;
using System.Text;
using Wallet.Application.Interfaces; // 🔹 Importante para enxergar a Interface

namespace Wallet.Infrastructure.Services
{
    // 🔹 Adicionado ": ISenhaService" para assinar o contrato
    public class SenhaService : ISenhaService 
    {
        // 🔹 Renomeado para HashPassword para bater com o que o UsuarioService pede
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        // 🔹 Adicionado o VerifyPassword que a interface e o Service provavelmente pedem
        public bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}