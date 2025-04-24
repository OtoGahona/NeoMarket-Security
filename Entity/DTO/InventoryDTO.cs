using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
    public  class InventoryDto
    {
        public int Id { get; set; }
        public string NameInventory { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string DescriptionInvetory { get; set; } = string.Empty;
        public string ZoneProduct {  get; set; } = string.Empty;
        public string Observations { get; set; }
    }
}
