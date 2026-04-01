using Wallet.Domain.Entities;
using Wallet.Application.Interfaces;

namespace Wallet.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IWalletRepository _walletRepository; // 🔹 Adicionado
        private readonly ISenhaService _senhaService;

        // 🔹 Construtor atualizado para receber o repositório de Wallet
        public UsuarioService(
            IUsuarioRepository repository, 
            IWalletRepository walletRepository, 
            ISenhaService senhaService)
        {
            _repository = repository;
            _walletRepository = walletRepository;
            _senhaService = senhaService;
        }

        public async Task<Usuario> Registrar(string username, string password)
        {
            // 1. Validação de existência
            var existing = await _repository.GetByUsernameAsync(username);
            if (existing != null)
                throw new InvalidOperationException("Usuário já existe");

            // 2. Criptografia
            var hashedPassword = _senhaService.HashPassword(password);
            
            // 3. Criação do Usuário
            var usuario = new Usuario(username);
            usuario.SetSenha(hashedPassword);
            var usuarioCriado = await _repository.CreateAsync(usuario);

            // 4. 🔹 CRIAÇÃO AUTOMÁTICA DA CARTEIRA
            // Usamos o ID que o banco acabou de gerar para o usuário
            var novaWallet = new Wallet.Domain.Entities.Wallet 
            { 
                Id = Guid.NewGuid(),
                UsuarioId = usuarioCriado.Id, 
                Balance = 0.00m,
                CreatedAt = DateTime.UtcNow 
            };

            await _walletRepository.CreateAsync(novaWallet);

            return usuarioCriado;
        }

        public async Task<Usuario> Login(string username, string password)
        {
            var usuario = await _repository.GetByUsernameAsync(username);
            if (usuario == null) return null;

            if (!_senhaService.VerifyPassword(password, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Senha inválida");

            return usuario;
        }

        public async Task Deletar(Guid id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario != null) await _repository.DeleteAsync(id);
        }

        public async Task AlterarSenha(Guid id, string novaSenha)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) throw new InvalidOperationException("Usuário não encontrado");

            var novoHash = _senhaService.HashPassword(novaSenha);
            usuario.SetSenha(novoHash);

            await _repository.UpdateAsync(usuario);
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(Guid id) => await _repository.GetByIdAsync(id);
        public async Task<Usuario> ObterUsuarioPorUsernameAsync(string username) => await _repository.GetByUsernameAsync(username);
    }
}
