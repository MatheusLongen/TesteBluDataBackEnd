using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesteBluData.Data;
using TesteBluData.Models;

namespace TesteBluData.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelefoneController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelefoneController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Telefone>>> GetTelefones()
        {
            return await _context.Telefones.Include(t => t.Fornecedor).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Telefone>> GetTelefone(int id)
        {
            var telefone = await _context.Telefones.Include(t => t.Fornecedor).FirstOrDefaultAsync(t => t.Id == id);
            if (telefone == null) return NotFound();
            return telefone;
        }

        [HttpPost]
        public async Task<ActionResult<Telefone>> PostTelefone(Telefone telefone)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(telefone.FornecedorId);
            if (fornecedor == null)
                return BadRequest("Fornecedor inválido.");

            _context.Telefones.Add(telefone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTelefone), new { id = telefone.Id }, telefone);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTelefone(int id, Telefone telefone)
        {
            if (id != telefone.Id) return BadRequest();

            var fornecedor = await _context.Fornecedores.FindAsync(telefone.FornecedorId);
            if (fornecedor == null)
                return BadRequest("Fornecedor inválido.");

            _context.Entry(telefone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Telefones.Any(t => t.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTelefone(int id)
        {
            var telefone = await _context.Telefones.FindAsync(id);
            if (telefone == null) return NotFound();

            _context.Telefones.Remove(telefone);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
