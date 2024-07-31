using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFConfin.Data;
using WFConfin.Models;

namespace WFConfin.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class CidadeController : Controller
    {
        private readonly WFConfinDbContext _context;

        public CidadeController(WFConfinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCidades()
        {
            try
            {
                var result = _context.Cidade.ToList();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de cidades. Exceção:{ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostCidade([FromBody] Cidade cidade)
        {
            try
            {
                await _context.Cidade.AddAsync(cidade);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, cidade Incluido");
                }
                else
                {
                    return BadRequest($"Erro, cidade não incluido");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, cidade não incluido. Exeção: {e.Message}");
            }

        }

        [HttpPut]
        public async Task <ActionResult> PutCidade([FromBody] Cidade cidade)
        {
            try
            {
                _context.Cidade.Update(cidade);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, cidade alterada");
                }
                else
                {
                    return BadRequest($"Erro, cidade não alterada");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, Cidade não alterada. Exeção: {e.Message}");
            }

        }

        [HttpDelete("{id}")]
        public async Task <ActionResult> DeleteCidade([FromRoute] Guid id)
        {
            try
            {
                var cidade = await _context.Cidade.FindAsync(id);

                

                    _context.Cidade.Remove(cidade);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso, cidade Excluída");
                    }
                    else
                    {
                        return BadRequest($"Erro, cidade não Excluída");
                    }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, cidade não alterada. Exeção: {e.Message}");
            }

        }

        [HttpGet("{cidadeFind}")]
        public async Task <ActionResult> GetEstado([FromRoute] string cidadeFind)
        {
            try
            {
                var cidade = await _context.Cidade.FindAsync(cidadeFind);

                if (cidade.Nome== cidadeFind && !string.IsNullOrEmpty(cidade.Nome))
                {

                    return Ok(cidade);


                }
                else
                {
                    return NotFound("Erro, cidade não existe");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, consulta de cidade. Exeção: {e.Message}");
            }

        }

        [HttpGet("Pesquisa")]
        public async Task <ActionResult> GetCidadePesquisa([FromQuery] string valor)
        {
            try
            {
                var lista = from o in _context.Cidade.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;
                return Ok(lista);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de cidade. Exeção: {e.Message}");
            }

        }

        [HttpGet("Paginacao")]
        public async Task <ActionResult> GetCidadePaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Cidade.ToList()
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

                var paginacaoResponse = new PaginacaoResponse<Cidade>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de cidade. Exeção: {e.Message}");
            }

        }
    }
}
