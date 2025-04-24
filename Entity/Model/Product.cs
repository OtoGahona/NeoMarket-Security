using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string NameProduct { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool Status { get; set; }
        public int IdInventory { get; set; }
        public Inventory Inventory { get; set; }
        public int IdCategory { get; set; }
        public Category Category { get; set; }
        public int IdImageItem { get; set; }
        public ImageItem ImageItems { get; set; }
        public ICollection<MovimientInventory> MovimientInventories { get; set; }
        public ICollection<Buyout> Buyouts { get; set; }
        public ICollection<SaleDetail> SeleDetails { get; set; }


    }
}
