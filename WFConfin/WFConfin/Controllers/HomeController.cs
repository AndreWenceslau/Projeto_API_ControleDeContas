using Microsoft.AspNetCore.Mvc;
using WFConfin.Models;

namespace WFConfin.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {

        private static List<Estado> listaEstados = new List<Estado>();
        [HttpGet("estado")]
        public IActionResult GetEstados()
        {
             return Ok(listaEstados);
        }
        [HttpPost("estado")]
        public IActionResult PostEstados([FromBody] Estado estado)
        {
            listaEstados.Add(estado);
            return Ok(listaEstados);
        
        }

        [HttpGet]
        public IActionResult GetInformacao()
        {
            var result = "Retorno em texto";
            return Ok(result);
        }

        [HttpGet("info2")]
        public IActionResult GetInformacao2()
        {
            var result = "Retorno em texto 2";
            return Ok(result);
        }

        [HttpGet("info3/{valor}")]
        public IActionResult GetInformacao3(string valor)
        {
            var result = "Retorno em texto 3 -valor:" + valor;
            return Ok(result);
        }

        [HttpPost("info4")]
        public IActionResult GetInformacao4([FromQuery] string valor)
        {
            var result = "Retorno em texto 4 -valor:" + valor;
            return Ok(result);
        }

        [HttpGet("info5")]
        public IActionResult GetInformacao5([FromHeader] string valor)
        {
            var result = "Retorno em texto 5 -valor:" + valor;
            return Ok(result);
        }
    }

}
