using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using NovaApi.Models;

namespace NovaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        // Rota GET para obter a lista de pessoas do arquivo JSON
        [HttpGet]
        public async Task<IActionResult> GetPessoas()
        {
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pessoas.json");

            if (!System.IO.File.Exists(jsonFilePath))
            {
                return NotFound("Arquivo pessoas.json não encontrado.");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            var pessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonData);

            return Ok(pessoas);
        }
    }
}
