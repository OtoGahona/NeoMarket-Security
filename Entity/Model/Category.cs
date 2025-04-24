using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    namespace Entity.Model
{
	public class Category
	{
		public int Id { get; set; }
		public string NameCategory { get; set; } = string.Empty;
        public string Description { get; set; }
        public Product Product { get; set; }

    }
}
