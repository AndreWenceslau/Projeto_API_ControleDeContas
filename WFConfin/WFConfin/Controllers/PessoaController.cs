using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConfin.Data;
using WFConfin.Models;

namespace WFConfin.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PessoaController : Controller
    {
        private readonly WFConfinDbContext _context;

        public PessoaController(WFConfinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPessoas()
        {
            try
            {
                var result = await _context.Pessoa.ToListAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de pessoas. Exeção: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                await _context.Pessoa.AddAsync(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, pessoa incluida");
                }
                else
                {
                    return BadRequest($"Erro, pessoa não incluida");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, cidade não incluida. Exeção: {e.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                _context.Pessoa.Update(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, pessoa Alterada");
                }
                else
                {
                    return BadRequest($"Erro, pessoa não alterada");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, Pessoa não Alterada. Exeção: {e.Message}");
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePessoa([FromRoute] Guid id)
        {
            try
            {
                var pessoa = await _context.Pessoa.FindAsync(id);



                _context.Pessoa.Remove(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, pessoa excluída");
                }
                else
                {
                    return BadRequest($"Erro, pessoa não excluída");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pessoa não alterada. Exeção: {e.Message}");
            }

        }

        [HttpGet("{pessoaFind}")]
        public async Task<ActionResult> GetPessoa([FromRoute] string pessoaFind)
        {
            try
            {
                var pessoa = await _context.Pessoa.FindAsync(pessoaFind);

                if (pessoa.Nome == pessoaFind && !string.IsNullOrEmpty(pessoa.Nome))
                {

                    return Ok(pessoa);


                }
                else
                {
                    return NotFound("Erro, pessoa não existe");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, consulta de pessoa. Exeção: {e.Message}");
            }

        }

        [HttpGet("Pesquisa")]
        public async Task<ActionResult> GetPessoaPesquisa([FromQuery] string valor)
        {
            try
            {
                var lista = from o in _context.Pessoa.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;
                return Ok(lista);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de pessoa. Exeção: {e.Message}");
            }

        }

        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetPessoaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Pessoa.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.Nome descending
                            select o;

                }
                else
                {
                    lista = from o in lista
                            orderby o.Nome ascending
                            select o;
                }
                var qtde = lista.Count();
                lista = lista
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var paginacaoResponse = new PaginacaoResponse<Pessoa>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de pessoa. Exeção: {e.Message}");
            }

        }
    }
}
