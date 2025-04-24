using Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
    public class RolFormDto

    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Permision Permision { get; set; }
        public int IdRol { get; set; }
        public int IdForm { get; set; }
    }
}
