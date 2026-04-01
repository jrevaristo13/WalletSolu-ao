using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Services;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UsuarioService _service;

    public AuthController(UsuarioService service)
    {
        _service = service;
    }

    [HttpPost("register")]
public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string senha)
{
    try 
    {
        var usuario = await _service.Registrar(username, senha);
        return Ok(new { mensagem = "Usuário criado com sucesso!", id = usuario.Id });
    }
    catch (InvalidOperationException ex)
    {
        // Em vez de erro 500, retorna um 400 amigável dizendo que o usuário já existe
        return BadRequest(new { erro = ex.Message });
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Erro inesperado: " + ex.Message);
    }
}

    [HttpPost("login")]
    // 🔹 Adicionado [FromQuery] aqui também
    public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string senha)
    {
        var result = await _service.Login(username, senha);
        
        if (result == null)
            return Unauthorized("Usuário ou senha inválidos.");

        return Ok(result);
    }
}