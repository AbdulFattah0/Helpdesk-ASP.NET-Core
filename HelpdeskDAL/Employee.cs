using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpdeskDAL;

public partial class Employee : HelpdeskEntity
{
   
    public string? Title { get; set; }
    

    

    public byte[]? Timer { get; set; }  // Keep this as byte[] for database concurrency

    public int? DepartmentId { get; set; }
    public new virtual Department? Department { get; set; }
    public bool? IsTech { get; set; }
    public byte[]? StaffPicture { get; set; }

    public virtual ICollection<Call> CallEmployees { get; set; } = new List<Call>();

    public virtual ICollection<Call> CallTeches { get; set; } = new List<Call>();

    public string? Email { get; set; } // Ensure this column exists in the database
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNo { get; set; }


}
