using System.Web.Mvc;

namespace EPTransporte.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Obtener la sucursal seleccionada de la sesión
            var sucursal = Session["SucursalSeleccionada"] as EPTransporte.Models.Sucursal;

            if (sucursal == null)
            {
                TempData["Message"] = "Por favor seleccione una sucursal";
                return RedirectToAction("Index", "Configuracion");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}