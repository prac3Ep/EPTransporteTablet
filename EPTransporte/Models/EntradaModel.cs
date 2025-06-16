using System;
using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class EntradaModel
    {
        public int Id { get; set; }

        [Display(Name = "Transportista")]
        [Required(ErrorMessage = "El transportista es requerido")]
        public int IdTransportista { get; set; }

        [Display(Name = "Operador")]
        [Required(ErrorMessage = "El operador es requerido")]
        public int IdOperador { get; set; }

        [Display(Name = "Económico")]
        [Required(ErrorMessage = "El económico es requerido")]
        public int IdEconomico { get; set; }

        public string SucursalNombre { get; set; }  // Solo el nombre, sin el ID

        [Display(Name = "Vacio")]
        public bool Vacio { get; set; }

        [Display(Name = "Cargado")]
        public bool Cargado { get; set; }

        [Display(Name = "Botando")]
        public bool Botando { get; set; }

        [Display(Name = "Número de Caja")]
        public string NumCaja { get; set; }

        [Display(Name = "Número de Sello")]
        public string NumSello { get; set; }

        public DateTime FechaEntrada { get; set; } = DateTime.Now;

        // Propiedades para mostrar nombres
        public string TransportistaNombre { get; set; }

        public string OperadorNombre { get; set; }
        public string EconomicoNombre { get; set; }
    }
}