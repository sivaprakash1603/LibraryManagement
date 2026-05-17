using System;
using System.Collections.Generic;
using LibraryManagementModelLibrary.Enums;

namespace LibraryManagementModelLibrary.Models;

public partial class Borrowing
{
    public int Id { get; set; }

    public int Memberid { get; set; }

    public string Barcodeno { get; set; } = null!;

    public DateOnly Borrowdate { get; set; }

    public DateOnly Duedate { get; set; }

    public DateOnly? Returndate { get; set; }

    public BorrowStatus Borrowstatus { get; set; }

    public virtual Bookcopy BarcodenoNavigation { get; set; } = null!;

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual Member Member { get; set; } = null!;

    public override string ToString()
    {
        return $"Borrowing ID: {Id}, Member ID: {Memberid}, Barcode: {Barcodeno}, Status: {Borrowstatus}, Borrowed: {Borrowdate}, Due: {Duedate}, Returned: {(Returndate.HasValue ? Returndate.Value.ToString() : "N/A")}";
    }
}
