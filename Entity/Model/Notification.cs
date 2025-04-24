using System;
using Entity.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public  class Notification
    {
        public int Id { get; set; }
        public TypeAction TypeAction { get; set; }
        public string Message { get; set; }
        public string Read { get; set; }
        public DateTime Date { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }

    }
}
