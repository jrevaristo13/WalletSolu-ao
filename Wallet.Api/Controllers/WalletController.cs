using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.DbContext; // 🔹 Importante adicionar este using

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly AppDbContext _context; // 🔹 Adicionado para gerenciar a Transaction

        // 🔹 Construtor atualizado com os 3 injetores
        public WalletController(
            IWalletRepository walletRepository, 
            ITransactionRepository transactionRepository,
            AppDbContext context) 
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _context = context;
        }

        [HttpGet("saldo/{usuarioId}")]
        public async Task<IActionResult> GetSaldo(Guid usuarioId)
        {
            var wallet = await _walletRepository.GetByUsuarioIdAsync(usuarioId);
            if (wallet == null) return NotFound(new { mensagem = "Carteira não encontrada." });

            return Ok(new { usuarioId = wallet.UsuarioId, saldo = wallet.Balance });
        }

        [HttpGet("extrato/{usuarioId}")]
        public async Task<IActionResult> GetExtrato(Guid usuarioId, [FromQuery] int dias = 30)
        {
            var transacoes = await _transactionRepository.GetByUsuarioIdAsync(usuarioId);

            if (transacoes == null || !transacoes.Any())
            {
                return Ok(new { mensagem = "Nenhum histórico encontrado.", historico = new List<object>() });
            }

            // Filtro de data para o extrato não ficar pesado
            var dataLimite = DateTime.UtcNow.AddDays(-dias);
            var historicoFiltrado = transacoes
                .Where(t => t.Data >= dataLimite)
                .OrderByDescending(t => t.Data);

            return Ok(new 
            { 
                usuarioId,
                quantidade = historicoFiltrado.Count(),
                historico = historicoFiltrado 
            });
        }

        [HttpPost("deposito")]
        public async Task<IActionResult> Deposito([FromQuery] Guid usuarioId, [FromQuery] decimal valor)
        {
            if (valor <= 0) 
                return BadRequest(new { erro = "O valor deve ser maior que zero." });

            var wallet = await _walletRepository.GetByUsuarioIdAsync(usuarioId);
            if (wallet == null) 
                return NotFound(new { erro = "Carteira não encontrada." });

            wallet.Balance += valor;
            wallet.CreatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);

            var novaTransacao = new Transacao($"Depósito de R$ {valor}", valor, "Receita");
            await _transactionRepository.CreateAsync(novaTransacao);

            return Ok(new { mensagem = "Depósito realizado!", novoSaldo = wallet.Balance });
        }

        [HttpPost("saque")]
        public async Task<IActionResult> Saque([FromQuery] Guid usuarioId, [FromQuery] decimal valor)
        {
            if (valor <= 0) 
                return BadRequest(new { erro = "O valor do saque deve ser maior que zero." });

            var wallet = await _walletRepository.GetByUsuarioIdAsync(usuarioId);
            if (wallet == null) return NotFound(new { erro = "Carteira não encontrada." });

            if (wallet.Balance < valor)
                return BadRequest(new { erro = "Saldo insuficiente." });

            wallet.Balance -= valor;
            wallet.CreatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);

            var transacaoSaque = new Transacao($"Saque em dinheiro de R$ {valor}", valor, "Despesa");
            await _transactionRepository.CreateAsync(transacaoSaque);

            return Ok(new { mensagem = "Saque realizado!", novoSaldo = wallet.Balance });
        }

        [HttpPost("transferir")]
        public async Task<IActionResult> Transferir([FromQuery] Guid origemId, [FromQuery] Guid destinoId, [FromQuery] decimal valor)
        {
            if (valor <= 0) return BadRequest("Valor inválido.");
            if (origemId == destinoId) return BadRequest("Contas iguais.");

            // 🛡️ Inicia a transação atômica (ACID)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try 
            {
                var walletOrigem = await _walletRepository.GetByUsuarioIdAsync(origemId);
                var walletDestino = await _walletRepository.GetByUsuarioIdAsync(destinoId);

                if (walletOrigem == null || walletDestino == null)
                    return NotFound("Uma das carteiras não existe.");

                if (walletOrigem.Balance < valor)
                    return BadRequest("Saldo insuficiente para transferência.");

                // Processa a troca de valores
                walletOrigem.Balance -= valor;
                walletDestino.Balance += valor;

                await _walletRepository.UpdateAsync(walletOrigem);
                await _walletRepository.UpdateAsync(walletDestino);

                // Registra os dois lados da moeda
                var hSaida = new Transacao($"Transferência enviada", valor, "Despesa");
                var hEntrada = new Transacao($"Transferência recebida", valor, "Receita");

                await _transactionRepository.CreateAsync(hSaida);
                await _transactionRepository.CreateAsync(hEntrada);

                // Só salva de verdade se tudo deu certo até aqui
                await transaction.CommitAsync();

                return Ok(new { mensagem = "PIX Seguro Realizado!", novoSaldo = walletOrigem.Balance });
            }
            catch (Exception)
            {
                // Se cair a internet ou der erro de banco, desfaz tudo automaticamente
                await transaction.RollbackAsync();
                return StatusCode(500, "Erro na transação. Seu saldo foi preservado.");
            }
        }
    }
}
