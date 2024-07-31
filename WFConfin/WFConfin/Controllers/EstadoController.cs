using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WFConfin.Data;
//using WFConfin.Migrations;
using WFConfin.Models;

namespace WFConfin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class EstadoController : Controller
    {
        private readonly WFConfinDbContext _context;

        public EstadoController(WFConfinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetEstados()
        {
            try
            {
                var result = _context.Estado.ToList();

                return Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro na listagem de estados. Exeção: {e.Message}");
            }

        }

        [HttpPost]
        public async Task <ActionResult> PostEstado([FromBody] Estado estado)
        {
            try
            {
                await _context.Estado.AddAsync(estado);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, estado Incluido");
                }
                else
                {
                    return BadRequest($"Erro, estado não incluido");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, estado não incluido. Exeção: {e.Message}");
            }

        }

        [HttpPut]
        public async Task <ActionResult> PutEstado([FromBody] Estado estado)
        {
            try
            {
                _context.Estado.Update(estado);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, estado alterado");
                }
                else
                {
                    return BadRequest($"Erro, estado não alterado");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, estado não alterado. Exeção: {e.Message}");
            }

        }

        [HttpDelete("{sigla}")]
        public async Task <ActionResult> DeleteEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);

                if (estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {

                    _context.Estado.Remove(estado);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso, estado excluído");
                    }
                    else
                    {
                        return BadRequest($"Erro, estado não excluído");
                    }


                }
                else
                {
                    return NotFound("Erro, estado não existe");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, estado não alterado. Exeção: {e.Message}");
            }

        }

        [HttpGet("{sigla}")]
        public async Task <ActionResult> GetEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);

                if (estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {

                    return Ok(estado);


                }
                else
                {
                    return NotFound("Erro, estado não existe");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, consulta de estado. Exeção: {e.Message}");
            }

        }
        [HttpGet("Pesquisa")]
        public async Task <ActionResult> GetEstadoPesquisa([FromQuery] string valor)
        {
            try
            {
                var lista = from o in _context.Estado.ToList()
                            where o.Sigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())   
                            select o;
                return Ok(lista);   

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de estado. Exeção: {e.Message}");
            }

        }

        [HttpGet("Paginacao")]
        public async Task <ActionResult> GetEstadoPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Estado.ToList()
                            where o.Sigla.ToUpper().Contains(valor.ToUpper())
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

                var paginacaoResponse = new PaginacaoResponse<Estado>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de estado. Exeção: {e.Message}");
            }

        }

    }
}
