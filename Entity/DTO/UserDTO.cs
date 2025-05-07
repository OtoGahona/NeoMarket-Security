using Entity.Model;

namespace Entity.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime? CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; } 
        public DateTime? DeleteAt { get; set; }
        public bool Status { get; set; }

        //public Person Person { get; set; }
        //public string NamePerson { get; set; }
        //public string NameRol { get; set; }
        //public string NameCompany { get; set; }
        public int IdPerson { get; set; }
        public int IdRol { get; set; }
        public int IdCompany { get; set; }
    }
}
