using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConfin.Data;
using WFConfin.Models;
using WFConfin.Services;

namespace WFConfin.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]

    public class UsuarioController : Controller
    {

        private readonly WFConfinDbContext _context;
        private readonly TokenService _service;

        public UsuarioController(WFConfinDbContext context, TokenService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult>Login([FromBody] UsuarioLogin usuarioLogin)
        {
            var usuario = await _context.Usuario.Where(X => X.Login == usuarioLogin.Login).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound("Usuário inváldo");
            }
            var passwordHash = MD5Hash.CaclHash(usuarioLogin.Password);

            if (usuario.Password != passwordHash)
            {
                return BadRequest("Senha inválida.");
            }



            var token = _service.GerarToken(usuario);
            usuario.Password = "";
            var result = new UsuarioResponse()
            {
                Usuario = usuario,
                Token = token
            };

            return Ok(result);

        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var result = await _context.Usuario.ToListAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de usuários. Exceção:{ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente,Empregado")]
        public async Task<ActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            try
            {
                var listUsuario = _context.Usuario.Where(x => x.Login == usuario.Login).ToList();
                if (listUsuario.Count > 0)
                {
                    return BadRequest("Erro, informação de Login inválido.");
                }
                string passwordHash = MD5Hash.CaclHash(usuario.Password);
                usuario.Password = passwordHash;
                await _context.Usuario.AddAsync(usuario);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, usuário Incluido");
                }
                else
                {
                    return BadRequest($"Erro, usuário não incluido");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, usuário não incluido. Exeção: {e.Message}");
            }

        }

        [HttpPut]
        public async Task<ActionResult>PutUsuario([FromBody] Usuario usuario)
        {
            try
            {
                string passwordHash = MD5Hash.CaclHash(usuario.Password);
                usuario.Password = passwordHash;
                _context.Usuario.Update(usuario);
                var valor = _context.SaveChanges();
                if (valor == 1)
                {
                    return Ok("Sucesso, usuário alterado");
                }
                else
                {
                    return BadRequest($"Erro, usuário não alterado");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, usuário não alterado. Exeção: {e.Message}");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<ActionResult> DeleteUsuario([FromRoute] Guid id)
        {
            try
            {
                var usuario = await _context.Usuario.FindAsync(id);



                _context.Usuario.Remove(usuario);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1)
                {
                    return Ok("Sucesso, usuário excluído");
                }
                else
                {
                    return BadRequest($"Erro, usuário não excluído");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, usuário não alterado. Exeção: {e.Message}");
            }

        }

        [HttpGet("{usuarioFind}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetUsuario([FromRoute] string usuarioFind)
        {
            try
            {
                var usuario = await _context.Usuario.FindAsync(usuarioFind);

                if (usuario.Nome == usuarioFind && !string.IsNullOrEmpty(usuario.Nome))
                {

                    return Ok(usuario);


                }
                else
                {
                    return NotFound("Erro, usuário não existe");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Cidade, consulta de usuário. Exeção: {e.Message}");
            }

        }

        [HttpGet("Pesquisa")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetUsuarioPesquisa([FromQuery] string valor)
        {
            try
            {
                var lista = from o in _context.Usuario.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;
                return Ok(lista);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de usuário. Exeção: {e.Message}");
            }

        }

        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetUsuarioPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Usuario.ToList()
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

                var paginacaoResponse = new PaginacaoResponse<Usuario>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de usuário. Exeção: {e.Message}");
            }

        }
    }
}
