using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Entity.Model
{
	public class Company
	{
		public int Id { get; set; }
		public DateTime CreateAt { get; set; }
		public DateTime UpdateAt { get; set; }
		public DateTime DeleteAt { get; set; }
		public string Description { get; set; }
		public string NameCompany { get; set; } = string.Empty;
		public string PhoneCompany { get; set; }
		public string Logo { get; set; }
		public string EmailCompany { get; set; } = string.Empty;
		public short NitCompany { get; set; }
		public bool Status { get; set; }
        public ICollection<Sede> Sede { get; set; }
    }
}
