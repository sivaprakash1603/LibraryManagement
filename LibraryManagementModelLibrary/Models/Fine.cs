using System;
using System.Collections.Generic;
using LibraryManagementModelLibrary.Enums;

namespace LibraryManagementModelLibrary.Models;

public partial class Fine
{
    public int Id { get; set; }

    public int Borrowingid { get; set; }

    public FineType Finetype { get; set; }

    public decimal Fineamount { get; set; }

    public bool Ispaid { get; set; }

    public virtual Borrowing Borrowing { get; set; } = null!;

    public virtual ICollection<Finepayment> Finepayments { get; set; } = new List<Finepayment>();

    public override string ToString()
    {
        return $"Fine ID: {Id}, Borrowing ID: {Borrowingid}, Type: {Finetype}, Amount: {Fineamount}, Paid: {Ispaid}";
    }
}
