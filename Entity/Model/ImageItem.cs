using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class ImageItem
    {
        public int Id { get; set; }
        public string UrlImage { get; set; }
        public bool Status { get; set; }
        public Product Product { get; set; }

    }
}
