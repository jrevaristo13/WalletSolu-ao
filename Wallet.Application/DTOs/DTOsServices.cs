using System;

namespace SistemaProduto.Services.Dtos
{
    public record ProdutoDto(Guid Id, string Nome, decimal Preco, int Quantidade);
}