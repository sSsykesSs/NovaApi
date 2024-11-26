using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using NovaApi.Models;
using System.Text.Json.Nodes;

namespace NovaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pessoas.json");

        
        [HttpGet]
        public async Task<IActionResult> GetPessoas()
        {
            if (!System.IO.File.Exists(_jsonFilePath))
            {
                return NotFound("Arquivo pessoas.json não encontrado.");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            var pessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonData);

            return Ok(pessoas);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pessoa pessoa)
        {
            if (pessoa == null)
            {
                return BadRequest("Dados da pessoa inválidos.");
            }


            pessoa.cpf = new string(pessoa.cpf.Where(char.IsDigit).ToArray());


            if (pessoa.cpf.Length != 11)
            {
                return BadRequest("O CPF deve conter exatamente 11 números.");
            }


            pessoa.cpf = $"{pessoa.cpf.Substring(0, 3)}.{pessoa.cpf.Substring(3, 3)}.{pessoa.cpf.Substring(6, 3)}-{pessoa.cpf.Substring(9)}";


            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            var pessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonData) ?? new List<Pessoa>();


            if (pessoas.Any(verificador => verificador.cpf == pessoa.cpf))
            {
                return Conflict($"Uma pessoa com o CPF {pessoa.cpf} já está cadastrada.");
            }


            pessoas.Add(pessoa);


            await System.IO.File.WriteAllTextAsync(_jsonFilePath,
                JsonSerializer.Serialize(pessoas, new JsonSerializerOptions { WriteIndented = true }));

            return Ok(pessoa);
        }

        [HttpGet("{cpf}")]
        public async Task<IActionResult> GetPessoaPorCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return BadRequest("O CPF não foi fornecido.");
            }

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
            {
                return BadRequest("O CPF deve conter exatamente 11 números.");
            }

            cpf = $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9)}";

            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            var pessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonData) ?? new List<Pessoa>();

            var pessoa = pessoas.FirstOrDefault(p => p.cpf == cpf);

            if (pessoa == null)
            {
                return NotFound($"Nenhuma pessoa encontrada com o CPF {cpf}.");
            }

            return Ok(pessoa);
        }

        [HttpPut("{cpf}")]
        public async Task<IActionResult> UpdatePessoa(string cpf, [FromBody] Pessoa dadosAtualizados)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return BadRequest("O CPF não foi fornecido.");
            }

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
            {
                return BadRequest("O CPF deve conter exatamente 11 números.");
            }

            cpf = $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9)}";

            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            var pessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonData) ?? new List<Pessoa>();

            var dadosExistentes = pessoas.FirstOrDefault(p => p.cpf == cpf);

            if (dadosExistentes == null)
            {
                return NotFound($"Nenhuma pessoa encontrada com o CPF {cpf}.");
            }

            dadosExistentes.nome = dadosAtualizados.nome ?? dadosExistentes.nome;
            dadosExistentes.idade = dadosAtualizados.idade != 0 ? dadosAtualizados.idade : dadosExistentes.idade;
            dadosExistentes.rg = dadosAtualizados.rg ?? dadosExistentes.rg;
            dadosExistentes.data_nasc = dadosAtualizados.data_nasc != default ? dadosAtualizados.data_nasc : dadosExistentes.data_nasc;
            dadosExistentes.sexo = dadosAtualizados.sexo ?? dadosExistentes.sexo;
            dadosExistentes.signo = dadosAtualizados.signo ?? dadosExistentes.signo;
            dadosExistentes.mae = dadosAtualizados.mae ?? dadosExistentes.mae;
            dadosExistentes.pai = dadosAtualizados.pai ?? dadosExistentes.pai;
            dadosExistentes.email = dadosAtualizados.email ?? dadosExistentes.email;
            dadosExistentes.cidade = dadosAtualizados.cidade ?? dadosExistentes.cidade;

            await System.IO.File.WriteAllTextAsync(_jsonFilePath,
                JsonSerializer.Serialize(pessoas, new JsonSerializerOptions { WriteIndented = true }));

            return Ok(dadosExistentes);
        }


        [HttpDelete("{cpf}")]
        public async Task<IActionResult> Delete(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return BadRequest("O CPF não foi fornecido.");
            }

           
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
            {
                return BadRequest("O CPF deve conter exatamente 11 números.");
            }

            
            cpf = $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9)}";

            
            var jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            var pessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonData) ?? new List<Pessoa>();

            
            var pessoaParaRemover = pessoas.FirstOrDefault(p => p.cpf == cpf);

            if (pessoaParaRemover == null)
            {
                return NotFound($"Nenhuma pessoa encontrada com o CPF {cpf}.");
            }

            
            pessoas.Remove(pessoaParaRemover);

            
            await System.IO.File.WriteAllTextAsync(_jsonFilePath,
                JsonSerializer.Serialize(pessoas, new JsonSerializerOptions { WriteIndented = true }));

            return Ok($"Pessoa com o CPF {cpf} foi removida com sucesso.");
        }
    }
}




