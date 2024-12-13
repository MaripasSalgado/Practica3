using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Practica3API.Models;
using System.Data;

namespace Practica3API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly IConfiguration _conf;
        private readonly IHostEnvironment _env;
        public ComprasController(IConfiguration conf, IHostEnvironment env)
        {
            _conf = conf;
            _env = env;
        }


        [HttpGet]
        [Route("ObtenerCompras")]
        public async Task<IActionResult> ObtenerCompras()
        {
            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var procedure = "sp_ObtenerCompras";
                var compras = await context.QueryAsync<Principal>(procedure);
                return Ok(compras);
            }
        }

        [HttpGet]
        [Route("ObtenerComprasPendientes")]
        public async Task<IActionResult> GetComprasPendientes()
        {

            using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
            {
                var procedure = "sp_ObtenerComprasPendientes";
                var compraspendientes = await context.QueryAsync<Principal>(procedure);
                return Ok(compraspendientes);
            }
        }

 

        [HttpPost]
        [Route("RegistrarAbono")]
        public IActionResult RegistrarAbono(Abono model)
        {
            try {
                using (var context = new SqlConnection(_conf.GetSection("ConnectionStrings:DefaultConnection").Value))
                {
                    var respuesta = new Respuesta();
                    var result = context.Execute("sp_RegistrarAbono", new { model.IdCompra, model.Monto, model.Fecha });

                    if (result > 0)
                    {
                        respuesta.Codigo = 0;
                    }
                    else
                    {
                        respuesta.Codigo = -1;
                        respuesta.Mensaje = "Su información no se ha registrado correctamente";
                    }

                    return Ok(respuesta);
                }
            }
            catch (SqlException ex)
            {
                // Devuelve el mensaje de error generado en el SP
                return BadRequest(ex.Message);
            }


        }
    }

}
