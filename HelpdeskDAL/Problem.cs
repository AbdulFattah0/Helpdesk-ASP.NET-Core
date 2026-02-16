using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Problem : HelpdeskEntity
{
    //public int Id { get; set; }

    public string? Description { get; set; }

    public byte[] Timer { get; set; } = null!;

    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}
