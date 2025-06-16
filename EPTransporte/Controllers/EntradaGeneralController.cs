using System;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class EntradaGeneralController : Controller
    {
        // GET: EntradaGeneral/Create
        public ActionResult RegistroEntradaGeneral()
        {
            // Obtener la sucursal seleccionada de la sesión
            var sucursal = Session["SucursalSeleccionada"] as EPTransporte.Models.Sucursal;

            if (sucursal == null)
            {
                TempData["ErrorMessage"] = "Por favor seleccione una sucursal primero";
                return RedirectToAction("Index", "Configuracion");
            }

            var model = new EntradaGeneralModel();
            return View(model);
        }

        // POST: EntradaGeneral/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistroEntradaGeneral(EntradaGeneralModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la sucursal seleccionada de la sesión
                    var sucursal = Session["SucursalSeleccionada"] as EPTransporte.Models.Sucursal;

                    if (sucursal == null)
                    {
                        TempData["ErrorMessage"] = "Por favor seleccione una sucursal primero";
                        return RedirectToAction("Index", "Configuracion");
                    }

                    // Asignar el nombre de la sucursal al modelo
                    model.SitioEP = sucursal.SitioEP; // Asegúrate que la propiedad se llame "Nombre" en tu modelo Sucursal

                    // Validar que al menos un estado esté seleccionado
                    if (!model.Vacio && !model.Cargado && !model.Botando)
                    {
                        ModelState.AddModelError("", "Debe seleccionar al menos un estado (Vacío, Cargado o Botando)");
                        return View(model);
                    }

                    // Guardar en la base de datos
                    GuardarEntradaGeneral(model);
                    TempData["SuccessMessage"] = "Entrada creada ✓";

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar la entrada: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult GuardarEntradaGeneral(EntradaGeneralModel model)
        {
            try
            {
                // Obtener la sucursal de la sesión por si acaso
                var sucursal = Session["SucursalSeleccionada"] as EPTransporte.Models.Sucursal;
                if (sucursal != null && string.IsNullOrEmpty(model.SitioEP))
                {
                    // Asignar solo el nombre si no está establecido
                    model.SitioEP = sucursal.SitioEP;
                }

                Conexion.GuardarEntradaGeneral(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}