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
            public string Observation { get; set; }
            public DateTime? CreateAt { get; set; } = DateTime.Now;
            public DateTime? DeleteAt { get; set; } = DateTime.Now;
            public DateTime? UpdateAt { get; set; } = DateTime.Now;     
            public string DescriptionInventory { get; set; } = string.Empty;
            public string ZoneProduct { get; set; } = string.Empty;
            public ICollection<MovimientInventory> MovimientInventories { get; set; }
            public ICollection<Product> Products { get; set; }


    }

}
