using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Practica3Web.Models;

namespace Practica3Web.Controllers
{
    public class ComprasController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _conf;
        public ComprasController(IHttpClientFactory http, IConfiguration conf)
        {
            _http = http;
            _conf = conf;
        }

        [HttpGet]
        public async Task<IActionResult> Consulta()
        {
            List<Principal> compras = new List<Principal>();
            using (var client = _http.CreateClient())
            {
                var url = _conf.GetSection("Variables:UrlApi").Value + "Compras/ObtenerCompras";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    compras = await response.Content.ReadFromJsonAsync<List<Principal>>();
                }
                else
                {
                    ViewBag.Error = "No se pudieron obtener las compras.";
                }
            }

            return View(compras);
        }

    }
}


