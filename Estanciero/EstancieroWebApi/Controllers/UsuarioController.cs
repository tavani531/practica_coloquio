using EstancieroEntities;
using EstancieroRequests;
using EstancieroResponses;
using EstancieroService;
using Microsoft.AspNetCore.Mvc;

namespace EstancieroWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private Plataforma _plataformaService;

        public UsuarioController()
        {
            _plataformaService = new Plataforma();
        }

        [HttpPost]
        public IActionResult CrearUsuario([FromBody] CrearUsuarioRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = _plataformaService.CrearUsuario(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpGet]
        public IActionResult ObtenerUsuarios()
        {
            var usuarios = _plataformaService.ObtenerTodosLosUsuarios();
            return Ok(new ApiResponse<List<UsuariosEntities>>(usuarios, "Usuarios obtenidos correctamente"));
        }

        [HttpDelete("{dni}")]
        public IActionResult EliminarUsuario([FromRoute] int dni)
        {
            var response = _plataformaService.EliminarUsuario(dni);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }


    }
}
