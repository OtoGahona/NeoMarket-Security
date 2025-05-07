using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Entity.Model
	{
		public class User
		{
			public int Id { get; set; }
			public string UserName { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
			public DateTime? CreateAt { get; set; } 
		    public DateTime? UpdateAt { get; set; } 
            public DateTime? DeleteAt { get; set; }

            public bool Status { get; set; }
            public int IdPerson { get; set; }
            public Person Person { get; set; }
            public int IdRol { get; set; }
            public Rol Rol { get; set; }
            public int IdCompany { get; set; }
            public Company Company { get; set; }
            public ICollection<Buyout> Buyout { get; set; }
            public ICollection<Sale> Sele { get; set; }
            public ICollection<Notification> Notification { get; set; }

    }
}
