using System;
using System.Collections.Generic;
using LibraryManagementModelLibrary.Enums;

namespace LibraryManagementModelLibrary.Models;

public partial class Member
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public AccountStatus Accountstatus { get; set; }

    public int Membershiptypeid { get; set; }

    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    public virtual Membershiptype Membershiptype { get; set; } = null!;

    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Phone: {Phone}, Email: {Email}, Status: {Accountstatus}, Membership Type ID: {Membershiptypeid})";
    }
}
