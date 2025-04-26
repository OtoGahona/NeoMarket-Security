using Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class MovimientInventory
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public TypeMovement TypeMovement { get; set; }
        public int IdInventory { get; set; }
        public Inventory Inventory { get; set; }
        public int IdProduct { get; set; }
        public Product Product { get; set; }

    }
}
