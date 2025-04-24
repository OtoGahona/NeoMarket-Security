using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
     public class SedeDto
    {
        public int Id { get; set; }
        public string NameSede { get; set; } = string.Empty;
        public string CodeSede { get; set; } = string.Empty;
        public string AddressSede { get; set; } = string.Empty;
        public int PhoneSede { get; set; }
        public string EmailSede { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int IdCompany { get; set; }

    }
}
