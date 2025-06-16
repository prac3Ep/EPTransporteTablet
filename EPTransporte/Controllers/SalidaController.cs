using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class SalidaController : Controller
    {
        [HttpPost]
        public ActionResult ActualizarFechaSalida(int idSalida)
        {
            try
            {
                Conexion.ActualizarFechaSalida(idSalida);
                TempData["SuccessMessage"] = "Salida aceptada ✓";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al registrar" + ex.Message;
            }

            return RedirectToAction("Index", "Home", new { id = idSalida });
        }

        [HttpGet]
        public ActionResult EditarSalida(int id)
        {
            var oSalida = Conexion.ObtenerSalidaEditar(id);

            if (oSalida == null)
            {
                return HttpNotFound();
            }

            // Cargar dropdowns
            var sucursalesData = Conexion.ObtenerSucursalesDisponibles();
            ViewBag.Sucursales = new SelectList(
                sucursalesData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["SitioEP"].ToString(),
                    Text = row["SitioEP"].ToString()
                }),
                "Value", "Text", oSalida.SucursalEP);

            var transportistasData = Conexion.ObtenerTransportistas();
            ViewBag.Transportistas = new SelectList(
                transportistasData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["EmpresaContratista"].ToString(),
                    Text = row["EmpresaContratista"].ToString()
                }),
                "Value", "Text", oSalida.Transportista);

            var auditoresData = Conexion.ObtenerUsuarios();
            ViewBag.Auditores = new SelectList(
                auditoresData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["Nombre"].ToString(),
                    Text = row["Nombre"].ToString()
                }),
                "Value", "Text", oSalida.Auditor);

            // Cargar operadores y económicos del transportista actual
            if (!string.IsNullOrEmpty(oSalida.Transportista))
            {
                var operadoresData = Conexion.ObtenerOperadoresPorTransportistaNombre(oSalida.Transportista);
                ViewBag.Operadores = new SelectList(
                    operadoresData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["NombreOperador"].ToString(),
                        Text = row["NombreOperador"].ToString()
                    }),
                    "Value", "Text", oSalida.Operador);

                var economicosData = Conexion.ObtenerEconomicosPorTransportistaNombre(oSalida.Transportista);
                ViewBag.Economicos = new SelectList(
                    economicosData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["Economico"].ToString(),
                        Text = row["Economico"].ToString()
                    }),
                    "Value", "Text", oSalida.Economico);
            }

            return View(oSalida);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarEditar(EditSalida model)
        {
            if (!ModelState.IsValid)
            {
                // Reconstruir dropdowns si hay error de validación
                var sucursalesData = Conexion.ObtenerSucursalesDisponibles();
                ViewBag.Sucursales = new SelectList(
                    sucursalesData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["SitioEP"].ToString(),
                        Text = row["SitioEP"].ToString()
                    }),
                    "Value", "Text", model.SucursalEP);

                var transportistasData = Conexion.ObtenerTransportistas();
                ViewBag.Transportistas = new SelectList(
                    transportistasData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["EmpresaContratista"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }),
                    "Value", "Text", model.Transportista);

                if (!string.IsNullOrEmpty(model.Transportista))
                {
                    var operadoresData = Conexion.ObtenerOperadoresPorTransportistaNombre(model.Transportista);
                    ViewBag.Operadores = new SelectList(
                        operadoresData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["NombreOperador"].ToString(),
                            Text = row["NombreOperador"].ToString()
                        }),
                        "Value", "Text", model.Operador);

                    var economicosData = Conexion.ObtenerEconomicosPorTransportistaNombre(model.Transportista);
                    ViewBag.Economicos = new SelectList(
                        economicosData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["Economico"].ToString(),
                            Text = row["Economico"].ToString()
                        }),
                        "Value", "Text", model.Economico);
                }

                return View(model);
            }

            try
            {
                Conexion.GuardarEditar(model);
                TempData["SuccessMessage"] = "Salida actualizada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar la salida: " + ex.Message);

                // Reconstruir dropdowns en caso de error
                var sucursalesData = Conexion.ObtenerSucursalesDisponibles();
                ViewBag.Sucursales = new SelectList(
                    sucursalesData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["SitioEP"].ToString(),
                        Text = row["SitioEP"].ToString()
                    }),
                    "Value", "Text", model.SucursalEP);

                var transportistasData = Conexion.ObtenerTransportistas();
                ViewBag.Transportistas = new SelectList(
                    transportistasData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["EmpresaContratista"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }),
                    "Value", "Text", model.Transportista);

                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetOperadoresByTransportistaNombre(string transportistaNombre)
        {
            try
            {
                if (string.IsNullOrEmpty(transportistaNombre))
                {
                    return Json(new { error = "Nombre de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var operadores = Clases.Conexion.ObtenerOperadoresPorTransportistaNombre(transportistaNombre);

                var operadoresList = operadores.AsEnumerable().Select(row => new
                {
                    Nombre = row.Field<string>("NombreOperador"),
                    Licencia = row.Field<string>("Licencia") ?? string.Empty
                }).ToList();

                return Json(operadoresList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en GetOperadoresByTransportistaNombre: {ex}");
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetEconomicosByTransportistaNombre(string transportistaNombre)
        {
            try
            {
                if (string.IsNullOrEmpty(transportistaNombre))
                {
                    return Json(new { error = "Nombre de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var economicos = Clases.Conexion.ObtenerEconomicosPorTransportistaNombre(transportistaNombre);

                var economicosList = economicos.AsEnumerable().Select(row => new
                {
                    Nombre = row.Field<string>("Economico")
                }).ToList();

                return Json(economicosList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en GetEconomicosByTransportistaNombre: {ex}");
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Métodos AJAX para cargar operadores y economicos
        [HttpGet]
        public JsonResult GetOperadoresByTransportista(int idTransportista)
        {
            try
            {
                if (idTransportista <= 0)
                {
                    return Json(new { error = "ID de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var operadores = Clases.Conexion.ObtenerOperadoresPorTransportista(idTransportista);

                // Convertir DataTable
                var operadoresList = operadores.AsEnumerable().Select(row => new
                {
                    Id = row.Field<int>("Id"),
                    NombreOperador = row.Field<string>("NombreOperador"),
                    Licencia = row.Field<string>("Licencia") ?? string.Empty
                }).ToList();

                return Json(operadoresList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Retornar error controlado con código 200 pero con informacion
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetEconomicosByTransportista(int idTransportista)
        {
            try
            {
                if (idTransportista <= 0)
                {
                    return Json(new { error = "ID de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var economicos = Clases.Conexion.ObtenerEconomicosPorTransportista(idTransportista);

                var economicosList = economicos.AsEnumerable().Select(row => new
                {
                    Id = row.Field<int>("Id"),
                    Economico = row.Field<string>("Economico")
                }).ToList();

                return Json(economicosList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Index()
        {
            // Obtener la sucursal seleccionada de la sesión

            if (!(Session["SucursalSeleccionada"] is EPTransporte.Models.Sucursal sucursal))
            {
                TempData["ErrorMessage"] = "Por favor seleccione una sucursal primero";
                return RedirectToAction("Index", "Configuracion");
            }

            // Obtener solo las salidas de la sucursal seleccionada
            List<DatosSalida> oSalida = Clases.Conexion.ObtenerSalidasPendientesPorSucursal(sucursal.SitioEP);
            return View(oSalida);
        }
    }
}