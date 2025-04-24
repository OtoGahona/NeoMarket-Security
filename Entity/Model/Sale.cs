using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Totaly { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }
        public ICollection<SaleDetail> SeleDetails { get; set; }
    }
}
