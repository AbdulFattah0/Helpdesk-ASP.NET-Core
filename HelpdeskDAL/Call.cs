using HelpdeskDAL;
using System.Text.Json.Serialization;

public partial class Call : HelpdeskEntity
{
  
    public int EmployeeId { get; set; }  // Foreign Key to Employee
    public int ProblemId { get; set; }   // Foreign Key to Problem
    public int TechId { get; set; }      // Foreign Key to Tech (Employee)


    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }
    public bool OpenStatus { get; set; }
    public string Notes { get; set; } = string.Empty;

    public byte[]? Timer { get; set; }
    
    // Navigation Properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual Problem Problem { get; set; } = null!;
    public virtual Employee Tech { get; set; } = null!;
}
