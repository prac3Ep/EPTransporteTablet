using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class EntradaGeneralModel
    {
        [Display(Name = "Transportista")]
        [Required(ErrorMessage = "El nombre del transportista es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string TransportistaNombre { get; set; }

        [Display(Name = "Operador")]
        [Required(ErrorMessage = "El nombre del operador es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string OperadorNombre { get; set; }
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]


        [Display(Name = "Económico")]
        [Required(ErrorMessage = "El número económico es requerido")]
        public string EconomicoNombre { get; set; }

        public string SitioEP { get; set; }

        [Display(Name = "Vacío")]
        public bool Vacio { get; set; }

        [Display(Name = "Cargado")]
        public bool Cargado { get; set; }

        [Display(Name = "Botando")]
        public bool Botando { get; set; }

        [Display(Name = "Número de Caja")]
        public string NumCaja { get; set; }

        [Display(Name = "Número de Sello")]
        public string NumSello { get; set; }
    }
}