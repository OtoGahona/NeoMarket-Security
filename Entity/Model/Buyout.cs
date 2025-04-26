using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Buyout
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int IdProduct { get; set; }
        public Product Product { get; set; }
        public int  IdUser { get; set; }
        public User User { get; set; }


    }
}
