﻿using Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	namespace Entity.Model
	{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public string PhoneNumber { get; set; } 
        public TypeIdentification? TypeIdentification { get; set; }
        public bool Status { get; set; }
        public int NumberIdentification { get; set; }
        public User User { get; set; }
    }
}
