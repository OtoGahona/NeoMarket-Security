using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
     public  class ImageItemDTO
    {
        public int Id { get; set; }
        public string UrlImage { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
