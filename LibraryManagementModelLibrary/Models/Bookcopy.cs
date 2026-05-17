using System;
using System.Collections.Generic;
using LibraryManagementModelLibrary.Enums;

namespace LibraryManagementModelLibrary.Models;

public partial class Bookcopy
{
    public string Barcodeno { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public BookcopyStatus Status { get; set; }

    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    public virtual Book IsbnNavigation { get; set; } = null!;

    public override string ToString()
    {
        return $"Barcode: {Barcodeno}, ISBN: {Isbn}, Status: {Status}";
    }
}
