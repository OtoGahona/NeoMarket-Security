using System;
using Entity.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public TypeIdentification? TypeIdentification { get; set; }
        public int NumberIndification { get; set; }
        public bool Status { get; set; }
    }
}
