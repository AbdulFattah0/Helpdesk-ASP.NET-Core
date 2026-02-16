using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public abstract class HelpdeskEntity
    {
        public int Id { get; set; }
        //public string FirstName { get; set; } = string.Empty; // Initialized with a default value
        //public string LastName { get; set; } = string.Empty;  // Initialized with a default value
        //public string Email { get; set; } = string.Empty;     // Initialized with a default value
        //public string PhoneNo { get; set; } = string.Empty;   // Initialized with a default value


        //public Department Department { get; set; } = null!;   // Use null-forgiving operator for initialization

    }

}
