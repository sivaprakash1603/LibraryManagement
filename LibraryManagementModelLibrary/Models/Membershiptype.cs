using System;
using System.Collections.Generic;

namespace LibraryManagementModelLibrary.Models;

public partial class Membershiptype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Maximumborrowings { get; set; }

    public int Maximumborrowdays { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Max Borrowings: {Maximumborrowings}, Max Days: {Maximumborrowdays})";
    }
}
