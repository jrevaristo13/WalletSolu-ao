using System;

namespace Wallet.Domain.Entities
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public decimal Valor { get; private set; }
        public DateTime Data { get; private set; }
        public string Tipo { get; private set; } = string.Empty; // "Receita" ou "Despesa"
        public bool Status { get; private set; }

        // Construtor vazio para o Entity Framework
        public Transacao() { }

        // Construtor principal para criar novas transações
        public Transacao(string descricao, decimal valor, string tipo)
        {
            if (string.IsNullOrWhiteSpace(descricao)) 
                throw new ArgumentException("Descrição obrigatória");
            
            if (valor <= 0) 
                throw new ArgumentOutOfRangeException("O valor deve ser maior que zero");
            
            // Ajustei para aceitar os tipos que você definir
            if (tipo != "Receita" && tipo != "Despesa" && tipo != "Deposito") 
                throw new ArgumentException("Tipo de transação inválido");

            Id = Guid.NewGuid();
            Descricao = descricao;
            Valor = valor;
            Tipo = tipo;
            Data = DateTime.UtcNow;
            Status = true; // Como é um depósito direto, já nasce confirmado
        }

        public void ConfirmarPagamento() => Status = true;
    }
}