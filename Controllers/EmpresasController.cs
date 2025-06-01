using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesteBluData.Data;
using TesteBluData.Models;
using TesteBluData.Validates;

namespace TesteBluData.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetEmpresas()
        {
            return await _context.Empresas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Empresa>> GetEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();
            return empresa;
        }

        [HttpPost]
        public async Task<ActionResult<Empresa>> PostEmpresa(Empresa empresa)
        {
            if (!ValidadorCpfCnpj.ValidarCNPJ(empresa.CNPJ))
                return BadRequest(new { message = "CNPJ inválido." });

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmpresa), new { id = empresa.Id }, empresa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpresa(int id, Empresa empresa)
        {
            if (id != empresa.Id) return BadRequest();

            if (!ValidadorCpfCnpj.ValidarCNPJ(empresa.CNPJ))
                return BadRequest(new { message = "CNPJ inválido." });

            _context.Entry(empresa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Empresas.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpresa(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Fornecedores)
                    .ThenInclude(f => f.Telefones)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empresa == null) return NotFound();

            foreach (var fornecedor in empresa.Fornecedores)
            {
                _context.Telefones.RemoveRange(fornecedor.Telefones);
            }

            _context.Fornecedores.RemoveRange(empresa.Fornecedores);

            _context.Empresas.Remove(empresa);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}