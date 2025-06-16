using System.Linq;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class ConfiguracionController : Controller
    {
        public ActionResult Index()
        {
            var sucursales = Conexion.ObtenerSucursales();
            ViewBag.Sucursales = new SelectList(sucursales, "Id", "DisplayText");

            // Recuperar la sucursal seleccionada si existe
            var sucursalSeleccionada = Session["SucursalSeleccionada"] as Sucursal;
            ViewBag.SucursalSeleccionada = sucursalSeleccionada?.Id;

            return View();
        }

        [HttpPost]
        public ActionResult SeleccionarSucursal(int? sucursalId)
        {
            if (sucursalId == null || sucursalId <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una sucursal válida.";
                return RedirectToAction("Index"); // Cambiado de "SeleccionarSucursal" a "Index"
            }

            var sucursales = Conexion.ObtenerSucursales();
            var sucursalSeleccionada = sucursales.FirstOrDefault(s => s.Id == sucursalId.Value);

            if (sucursalSeleccionada == null)
            {
                TempData["ErrorMessage"] = "La sucursal seleccionada no existe.";
                return RedirectToAction("Index"); // Cambiado de "SeleccionarSucursal" a "Index"
            }

            Session["SucursalSeleccionada"] = sucursalSeleccionada;
            Session["SucursalId"] = sucursalId;
            TempData["SuccessMessage"] = "Sucursal seleccionada ✓";
            return RedirectToAction("Index", "Home");
        }
    }
}