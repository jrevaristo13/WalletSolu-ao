namespace Wallet.Application.Interfaces
{
    public interface ISenhaService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}