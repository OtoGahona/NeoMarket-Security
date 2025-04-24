using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class SaleDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int IdProduct { get; set; }
        public Product Product { get; set; }
        public int IdSele { get; set; }
        public Sale Sele { get; set; }
    }
}
