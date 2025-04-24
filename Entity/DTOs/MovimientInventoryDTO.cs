using System;
using Entity.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
    public class MovimientInventoryDto
    {
        public int Id { get; set; }
        public TypeMovement TypeMoviment { get; set; }
        public string Quantity { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int IdInventory { get; set; }
        public int IdProduct { get; set; }
    }
}
