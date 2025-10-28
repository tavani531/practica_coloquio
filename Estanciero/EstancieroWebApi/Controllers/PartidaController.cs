using Microsoft.AspNetCore.Mvc;
using EstancieroRequests;
using EstancieroResponses;
using EstancieroService;

namespace EstancieroWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidaController : ControllerBase
    {
        private Plataforma _plataformaService;

        public PartidaController()
        {
            _plataformaService = new Plataforma();
        }

        [HttpPost("partidas")]
        public IActionResult CrearPartida([FromBody] CrearPartidaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = _plataformaService.CrearPartida(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet("partidas/{idPartida}")]
        public IActionResult ObtenerPartida([FromRoute] int idPartida)
        {
            var response = _plataformaService.ObtenerPartida(idPartida);

            if (!response.Success)
            {
                return BadRequest(response); 
            }

            return Ok(response);
        }

        [HttpPost("partidas/{idPartida}/pausar")]
        public IActionResult PausarPartida([FromRoute] int idPartida)
        {
            var response = _plataformaService.PausarPartida(idPartida);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("partidas/{idPartida}/reanudar")]
        public IActionResult ReanudarPartida([FromRoute] int idPartida)
        {
            var response = _plataformaService.ReanudarPartida(idPartida);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("partidas/{idPartida}/suspender")]
        public IActionResult SuspenderPartida([FromRoute] int idPartida)
        {
            var response = _plataformaService.SuspenderPartida(idPartida);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("partidas/{idPartida}/lanzardado")]
        public IActionResult LanzarDado([FromRoute] int idPartida)
        {
            var response = _plataformaService.LanzarDado(idPartida);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
