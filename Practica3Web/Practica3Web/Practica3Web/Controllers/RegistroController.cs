using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Practica3Web.Models;

namespace Practica3Web.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _conf;
        public RegistroController(IHttpClientFactory http, IConfiguration conf)
        {
            _http = http;
            _conf = conf;
        }

        public async Task<IActionResult> Registro()
        {
            List<Principal> compras = new List<Principal>();
            using (var client = _http.CreateClient())
            {
                var url = _conf.GetSection("Variables:UrlApi").Value + "Compras/ObtenerComprasPendientes";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    compras = await response.Content.ReadFromJsonAsync<List<Principal>>();
                }
                else
                {
                    ViewBag.Error = "No se pudieron obtener las compras pendientes.";
                }
            }

            return View(compras);
        }

        [HttpPost]
        public async Task<IActionResult> Abonar(Abono abono)
        {
            using (var client = _http.CreateClient())
            {
                var url = _conf.GetSection("Variables:UrlApi").Value + "Compras/RegistrarAbono";

                try
                {
                    var response = await client.PostAsJsonAsync(url, abono);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Leer el mensaje de error devuelto por la API
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("Monto", errorMessage);

                        // Recargar las compras pendientes para mostrarlas en la vista Registro
                        List<Principal> compras = new List<Principal>();
                        var comprasUrl = _conf.GetSection("Variables:UrlApi").Value + "Compras/ObtenerComprasPendientes";
                        var comprasResponse = await client.GetAsync(comprasUrl);

                        if (comprasResponse.IsSuccessStatusCode)
                        {
                            compras = await comprasResponse.Content.ReadFromJsonAsync<List<Principal>>();
                        }

                        return View("Registro", compras);
                    }
                }
                catch (Exception ex)
                {
                    // Manejo general de errores
                    ModelState.AddModelError("", "Ocurrió un error al registrar el abono: " + ex.Message);
                    return RedirectToAction("Registro");
                }
            }

            // Redirigir a la vista de consulta si el registro fue exitoso
            return RedirectToAction("Consulta", "Compras");
        }
    }

}
