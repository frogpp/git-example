using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Config
    {
        public string Name { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
        public string IsActive { get; set; }
    }

    public class PhoneNumber
    {
        public string Tel { get; set; }

        public string Phone { get; set; }
    }
}
