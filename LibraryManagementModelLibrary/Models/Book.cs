using System;
using System.Collections.Generic;
namespace LibraryManagementModelLibrary.Models;

public partial class Book
{
    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Authorname { get; set; } = null!;

    public string? Edition { get; set; }

    public decimal Price { get; set; }

    public int Categoryid { get; set; }

    public virtual ICollection<Bookcopy> Bookcopies { get; set; } = new List<Bookcopy>();

    public virtual Bookcategory Category { get; set; } = null!;

    public override string ToString()
    {
        return $"{Title} by {Authorname} (ISBN: {Isbn})";
    }
}
