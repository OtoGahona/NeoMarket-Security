using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Enum
{
    public enum TypeIdentification
    {
        [Display(Name = "Cedula de Ciudadania")]
        CC = 1,

        [Display(Name = "Cedula Extrangera")]
        CE = 2,

        [Display(Name = "Tarjeta de Identidad")]
        TI = 3,

        [Display(Name = "Registro Civil")]
        RC = 4,

        [Display(Name = "Numero de Identificación Triburaria")]
        NIT = 5,
    }
}