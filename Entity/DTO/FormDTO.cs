using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class FormDto
    {
        public int Id { get; set; }
        public string NameForm { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int IdModule { get; set; }
    }
}
