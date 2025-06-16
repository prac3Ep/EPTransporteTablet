using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class EntradaController : Controller
    {
        private void LogError(string message, Exception ex = null)
        {
            string errorMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            if (ex != null)
            {
                errorMessage += $"\nException: {ex.ToString()}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nInner Exception: {ex.InnerException.ToString()}";
                }
            }

            // Escribe en el output de debug
            Debug.WriteLine(errorMessage);

            // Escribe en un archivo de log (asegúrate que la carpeta App_Data exista)
            string logPath = Server.MapPath("~/App_Data/ErrorLog.txt");
            System.IO.File.AppendAllText(logPath, errorMessage + "\n\n");

            // También escribe en el Event Viewer de Windows
            EventLog.WriteEntry("Application", errorMessage, EventLogEntryType.Error);
        }

        public ActionResult RegistroEntrada()
        {
            try
            {
                var sucursal = Session["SucursalSeleccionada"] as Sucursal;
                if (sucursal == null)
                {
                    TempData["ErrorMessage"] = "Por favor seleccione una sucursal primero";
                    return RedirectToAction("Index", "Configuracion");
                }

                var model = new EntradaModel();
                CargarListas();
                return View(model);
            }
            catch (Exception ex)
            {
                LogError("Error en RegistroEntrada (GET)", ex);
                TempData["ErrorMessage"] = "Ocurrió un error al cargar el formulario";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistroEntrada(EntradaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LogError("ModelState no válido en RegistroEntrada",
                             new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage))));
                    CargarListas();
                    return View(model);
                }

                var sucursal = Session["SucursalSeleccionada"] as Sucursal;
                if (sucursal == null)
                {
                    TempData["ErrorMessage"] = "Por favor seleccione una sucursal primero";
                    return RedirectToAction("Index", "Configuracion");
                }

                if (!model.Vacio && !model.Cargado && !model.Botando)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un estado (Vacío, Cargado o Botando)");
                    CargarListas();
                    return View(model);
                }

                // Obtener nombres con verificación de null
                try
                {
                    var transportista = Conexion.ObtenerTransportistaPorId(model.IdTransportista);
                    var operador = Conexion.ObtenerOperadorPorId(model.IdOperador);
                    var economico = Conexion.ObtenerEconomicoPorId(model.IdEconomico);

                    model.TransportistaNombre = transportista?["EmpresaContratista"]?.ToString() ?? "N/A";
                    model.OperadorNombre = operador?["NombreOperador"]?.ToString() ?? "N/A";
                    model.EconomicoNombre = economico?["Economico"]?.ToString() ?? "N/A";
                    model.SucursalNombre = sucursal.SitioEP;

                    Debug.WriteLine($"Datos a guardar: Transportista={model.TransportistaNombre}, " +
                                  $"Operador={model.OperadorNombre}, " +
                                  $"Económico={model.EconomicoNombre}, " +
                                  $"Sucursal={model.SucursalNombre}, " +
                                  $"Vacio={model.Vacio}, Cargado={model.Cargado}, Botando={model.Botando}");
                }
                catch (Exception ex)
                {
                    LogError("Error al obtener datos relacionados", ex);
                    throw;
                }

                // Guardar entrada con verificación
                try
                {
                    Conexion.GuardarEntrada(model);
                    TempData["SuccessMessage"] = "Entrada creada exitosamente ✓";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    LogError("Error al guardar en la base de datos", ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogError("Error general en RegistroEntrada (POST)", ex);
                ModelState.AddModelError("", $"Error al guardar la entrada. Detalles registrados.");
                CargarListas();
                return View(model);
            }
        }

        private void CargarListas()
        {
            try
            {
                ViewBag.Transportistas = Conexion.ObtenerTransportistas()
                    .AsEnumerable()
                    .Select(row => new SelectListItem
                    {
                        Value = row["Id"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }).ToList();
            }
            catch (Exception ex)
            {
                LogError("Error al cargar listas", ex);
                ViewBag.Transportistas = new List<SelectListItem>();
            }
        }

        public JsonResult GetOperadoresByTransportista(int idTransportista)
        {
            try
            {
                var operadores = Conexion.ObtenerOperadoresPorTransportista(idTransportista)
                    .AsEnumerable()
                    .Select(row => new
                    {
                        Id = row["Id"],
                        NombreOperador = row["NombreOperador"]
                    });

                return Json(operadores, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError($"Error en GetOperadoresByTransportista para idTransportista={idTransportista}", ex);
                return Json(new { error = "Error al cargar operadores" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetEconomicosByTransportista(int idTransportista)
        {
            try
            {
                var economicos = Conexion.ObtenerEconomicosPorTransportista(idTransportista)
                    .AsEnumerable()
                    .Select(row => new
                    {
                        Id = row["Id"],
                        Economico = row["Economico"]
                    });

                return Json(economicos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError($"Error en GetEconomicosByTransportista para idTransportista={idTransportista}", ex);
                return Json(new { error = "Error al cargar económicos" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}