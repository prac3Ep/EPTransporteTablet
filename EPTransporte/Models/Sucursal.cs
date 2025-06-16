using System;
using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    [Serializable]
    public class Sucursal
    {
        [Required(ErrorMessage = "Es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Es requerido")]
        public string SitioEP { get; set; }

        [Required(ErrorMessage = "Es requerido")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "Es requerido")]
        public bool Habilitado { get; set; }

        public string DisplayText => $"{SitioEP} - {Ubicacion}";
    }
}