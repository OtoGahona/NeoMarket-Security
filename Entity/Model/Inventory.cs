using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Entity.Model
{
        public class Inventory
        {

            public int Id { get; set; }
            public string NameInventory { get; set; }
            public bool Status { get; set; } 
            public string Observations { get; set; } = string.Empty;
            public DateTime? CreateAt { get; set; }
            public DateTime? DeleteAt { get; set; }
            public DateTime? UpdateAt { get; set; }     
            public string DescriptionInvetory { get; set; } = string.Empty;
            public string ZoneProduct { get; set; } = string.Empty;
            public ICollection<MovimientInventory> MovimientInventories { get; set; }
            public ICollection<Product> Products { get; set; }


    }

}
