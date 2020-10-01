using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VuelosNewShore.Models
{
    public class EntidadVuelo
    {
        [Required]
        [Display(Name ="Fecha de Vuelo")]
        [DataType(DataType.Date)]
        public DateTime From { get; set; }
        [Required]
        [Display(Name = "Origen")]
        public string Origin { get; set; }
        [Required]
        [Display(Name = "Destino")]
        public string Destination { get; set; }
    }
}