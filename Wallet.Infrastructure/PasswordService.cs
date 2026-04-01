using Microsoft.AspNetCore.Identity;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Security;

public class PasswordService
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string Hash(Usuario user, string senha)
        => _hasher.HashPassword(user, senha);

    public bool Verify(Usuario user, string senha)
        => _hasher.VerifyHashedPassword(user, user.SenhaHash, senha)
           == PasswordVerificationResult.Success;
}