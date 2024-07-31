using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConfin.Data;
using WFConfin.Models;

namespace WFConfin.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ContaController : Controller
    {

        private readonly WFConfinDbContext _context;

        public ContaController(WFConfinDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetContas()

        {
            try
            {
                var result = await _context.Conta.ToListAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao listas contas. Exeção: {ex.Message}"); 
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente,Empregado")]
        public async Task<ActionResult> PostConta([FromBody] Conta conta)
        {
            try
            {
                await _context.Conta.AddAsync(conta);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, conta Incluida");
                }
                else
                {
                    return BadRequest($"Erro, conta não incluida");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, conta não incluida. Exeção: {e.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<ActionResult> PutConta([FromBody] Conta conta)
        {
            try
            {
                _context.Conta.Update(conta);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, conta alterada");
                }
                else
                {
                    return BadRequest($"Erro, conta não alterada");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, conta não alterada. Exeção: {e.Message}");
            }

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<ActionResult> DeleteConta([FromRoute] Guid id)
        {
            try
            {
                var conta = await _context.Conta.FindAsync(id);



                _context.Conta.Remove(conta);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, conta excluída");
                }
                else
                {
                    return BadRequest($"Erro, conta não excluída");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, conta não alterada. Exeção: {e.Message}");
            }

        }
        [HttpGet("{pessoaFind}")]
        [Authorize(Roles = "Administrador,Gerente,Empregado")]
        public async Task<ActionResult> GetConta([FromRoute] string contaFind)
        {
            try
            {
                var conta = await _context.Conta.FindAsync(contaFind);

                if (conta.Descricao == contaFind && !string.IsNullOrEmpty(conta.Descricao))
                {

                    return Ok(conta);


                }
                else
                {
                    return NotFound("Erro, conta não existe");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, consulta de conta. Exeção: {e.Message}");
            }

        }

        [HttpGet("Pesquisa")]
        [Authorize(Roles = "Administrador,Gerente,Empregado")]
        public async Task<ActionResult> GetContaPesquisa([FromQuery] string valor)
        {
            try
            {
                var lista = from o in _context.Conta.ToList()
                            where o.Descricao.ToUpper().Contains(valor.ToUpper())
                            || o.Descricao.ToUpper().Contains(valor.ToUpper())
                            select o;
                return Ok(lista);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de conta. Exeção: {e.Message}");
            }

        }
        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetContaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Conta.ToList()
                            where o.Descricao.ToUpper().Contains(valor.ToUpper())
                            || o.Descricao.ToUpper().Contains(valor.ToUpper())
                            select o;

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.Descricao descending
                            select o;

                }
                else
                {
                    lista = from o in lista
                            orderby o.Descricao ascending
                            select o;
                }
                var qtde = lista.Count();
                lista = lista
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var paginacaoResponse = new PaginacaoResponse<Conta>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de conta. Exeção: {e.Message}");
            }

        }

    }
}
