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
        public DateTime? CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public string Description { get; set; } 
        public string NameCompany { get; set; } 
        public string PhoneCompany { get; set; }
        public string? Logo { get; set; }
        public string EmailCompany { get; set; } 
        public int NitCompany { get; set; } 
        public bool Status { get; set; }
    }
}