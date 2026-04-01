using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Services;

[Authorize]
[ApiController]
[Route("api/usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuarioController(UsuarioService service)
    {
        _service = service;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Deletar(id);
        return Ok();
    }

    [HttpPut("{id}/senha")]
    public async Task<IActionResult> AlterarSenha(Guid id, string senha)
    {
        await _service.AlterarSenha(id, senha);
        return Ok();
    }
}