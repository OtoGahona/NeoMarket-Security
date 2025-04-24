using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string NameCompany { get; set; } = string.Empty;
        public int PhoneCompany { get; set; } 
        public string Logo { get; set; } = string.Empty;
        public string EmailCompany { get; set; } = string.Empty;
        public string NitCompany { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
