using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Entity.Model
{
    public class Module
    {
        public int Id { get; set; }
        public string NameModule { get; set; } = string.Empty;
        public bool status { get; set; }
        public ICollection<Form> Forms { get; set; }

    }
}
