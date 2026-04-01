using System;

namespace Wallet.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string SenhaHash { get; private set; } = string.Empty;
        public string SenhaSalt { get; private set; } = string.Empty;
        public string? RefreshToken { get; private set; } // 👈 Faltava este
        public DateTime? RefreshTokenExpira { get; private set; } // 👈 Faltava este
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public Usuario(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username inválido");

            Id = Guid.NewGuid();
            Username = username;
        }

        // 🔐 Agora aceita os dois parâmetros que o Service está tentando enviar// No arquivo Usuario.cs (Domain)
public void SetSenha(string hash, string salt = "")
{
    if (string.IsNullOrWhiteSpace(hash))
        throw new ArgumentException("Hash da senha não pode ser vazio");

    SenhaHash = hash;
    SenhaSalt = salt;
}

        // ✅ Método que o Service está procurando na linha 42 do erro
        public void SetRefreshToken(string token, DateTime expira)
        {
            RefreshToken = token;
            RefreshTokenExpira = expira;
        }
    }
}