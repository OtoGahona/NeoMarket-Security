using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Entity.Model
{
	public class Sede
	{
        public int Id { get; set; } 
        public string NameSede { get; set; } = string.Empty;
        public string CodeSede { get; set; } = string.Empty;
        public string AddressSede { get; set; } = string.Empty;
        public string PhoneSede { get; set; } = string.Empty;
        public string EmailSede { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime DeleteAt { get; set; }
        public int IdCompany { get; set; }
        public Company Company { get; set; }


    }
}
