using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesteBluData.Data;
using TesteBluData.Models;

namespace TesteBluData.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FornecedoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fornecedor>>> GetFornecedores(
            [FromQuery] string? nome,
            [FromQuery] string? cpfCnpj,
            [FromQuery] DateTime? dataCadastroInicial,
            [FromQuery] DateTime? dataCadastroFinal)
        {
            var query = _context.Fornecedores
                .Include(f => f.Empresa)
                .Include(f => f.Telefones)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(f => f.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(cpfCnpj))
                query = query.Where(f => f.CPFouCNPJ.Contains(cpfCnpj));

            if (dataCadastroInicial.HasValue)
                query = query.Where(f => f.DataCadastro >= dataCadastroInicial.Value);

            if (dataCadastroFinal.HasValue)
                query = query.Where(f => f.DataCadastro <= dataCadastroFinal.Value);

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Fornecedor>> GetFornecedor(int id)
        {
            var fornecedor = await _context.Fornecedores
                .Include(f => f.Empresa)
                .Include(f => f.Telefones)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornecedor == null) return NotFound();

            return fornecedor;
        }

        [HttpPost]
        public async Task<ActionResult<Fornecedor>> PostFornecedor(Fornecedor fornecedor)
        {
            var empresa = await _context.Empresas.FindAsync(fornecedor.EmpresaId);
            if (empresa == null)
                return BadRequest("Empresa inválida.");

            if (string.Equals(empresa.UF?.Trim(), "PR", StringComparison.OrdinalIgnoreCase) && fornecedor.CPFouCNPJ.Length == 14)
            {
                if (!fornecedor.DataNascimento.HasValue)
                    return BadRequest(new { message = "Fornecedor pessoa física precisa informar Data de Nascimento" });

                var idade = DateTime.Today.Year - fornecedor.DataNascimento.Value.Year;
                if (fornecedor.DataNascimento.Value > DateTime.Today.AddYears(-idade)) idade--;

                if (idade < 18)
                    return BadRequest(new { message = "Fornecedor pessoa física menor de idade não pode ser cadastrado para essa empresa" });
            }

            if (fornecedor.CPFouCNPJ.Length == 14)
            {
                if (!ValidadorCpfCnpj.IsCpf(fornecedor.CPFouCNPJ))
                    return BadRequest(new { message = "CPF inválido." });

                if (string.IsNullOrEmpty(fornecedor.RG))
                    return BadRequest(new { message = "RG obrigatório para fornecedor pessoa física" });

                if (!fornecedor.DataNascimento.HasValue)
                    return BadRequest(new { message = "Data de nascimento obrigatória para fornecedor pessoa física" });
            }

            fornecedor.DataCadastro = fornecedor.DataCadastro.ToUniversalTime();

            if (fornecedor.DataNascimento.HasValue)
                fornecedor.DataNascimento = fornecedor.DataNascimento.Value.ToUniversalTime();

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFornecedor), new { id = fornecedor.Id }, fornecedor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFornecedor(int id, [FromBody] Fornecedor fornecedor)
        {
            if (id != fornecedor.Id) return BadRequest();

            var empresa = await _context.Empresas.FindAsync(fornecedor.EmpresaId);
            if (empresa == null)
                return BadRequest("Empresa inválida.");

            if (empresa.UF.Equals("PR", StringComparison.OrdinalIgnoreCase) && fornecedor.CPFouCNPJ.Length == 14)
            {
                if (!fornecedor.DataNascimento.HasValue)
                    return BadRequest(new { message = "Por favor, informe a data de nascimento do fornecedor" });

                var idade = DateTime.Today.Year - fornecedor.DataNascimento.Value.Year;
                if (fornecedor.DataNascimento.Value > DateTime.Today.AddYears(-idade)) idade--;

                if (idade < 18)
                    return BadRequest(new { message = "Fornecedor menor de 18 anos não pode ser cadastrado para essa empresa" });
            }

            if (fornecedor.CPFouCNPJ.Length == 14)
            {
                if (!ValidadorCpfCnpj.IsCpf(fornecedor.CPFouCNPJ))
                    return BadRequest(new { message = "CPF inválido." });

                if (string.IsNullOrEmpty(fornecedor.RG))
                    return BadRequest(new { message = "O RG é obrigatório para fornecedores pessoa física" });

                if (!fornecedor.DataNascimento.HasValue)
                    return BadRequest(new { message = "Por favor, informe a data de nascimento do fornecedor" });
            }

            fornecedor.DataCadastro = fornecedor.DataCadastro.ToUniversalTime();

            if (fornecedor.DataNascimento.HasValue)
                fornecedor.DataNascimento = fornecedor.DataNascimento.Value.ToUniversalTime();

            var fornecedorExistente = await _context.Fornecedores
                .Include(f => f.Telefones)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornecedorExistente == null)
                return NotFound();

            fornecedorExistente.Nome = fornecedor.Nome;
            fornecedorExistente.CPFouCNPJ = fornecedor.CPFouCNPJ;
            fornecedorExistente.PessoaFisica = fornecedor.PessoaFisica;
            fornecedorExistente.DataNascimento = fornecedor.DataNascimento;
            fornecedorExistente.DataCadastro = fornecedor.DataCadastro;
            fornecedorExistente.RG = fornecedor.RG;

            foreach (var telefoneExistente in fornecedorExistente.Telefones.ToList())
            {
                if (!fornecedor.Telefones.Any(t => t.Id == telefoneExistente.Id))
                    _context.Telefones.Remove(telefoneExistente);
            }

            foreach (var telefoneAtualizado in fornecedor.Telefones)
            {
                var telefoneExistente = fornecedorExistente.Telefones
                    .FirstOrDefault(t => t.Id == telefoneAtualizado.Id);

                if (telefoneExistente != null)
                {
                    telefoneExistente.Numero = telefoneAtualizado.Numero;
                }
                else
                {
                    fornecedorExistente.Telefones.Add(new Telefone
                    {
                        Numero = telefoneAtualizado.Numero
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Fornecedores.Any(f => f.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFornecedor(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null) return NotFound();

            var telefones = _context.Telefones.Where(t => t.FornecedorId == id);
            _context.Telefones.RemoveRange(telefones);

            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}