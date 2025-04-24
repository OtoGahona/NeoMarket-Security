using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entity.Enum
{
    public enum TypeMovement
    {
        [Display(Name = "Entrada de productos")]
        Entrada = 1,

        [Display(Name = "Salida de productos")]
        Salida = 2,
    }
}
