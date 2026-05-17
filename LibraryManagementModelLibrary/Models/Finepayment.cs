using System;
using System.Collections.Generic;

namespace LibraryManagementModelLibrary.Models;

public partial class Finepayment
{
    public int Id { get; set; }

    public int Fineid { get; set; }

    public decimal Amountpaid { get; set; }

    public DateTime Paymentdate { get; set; }

    public virtual Fine Fine { get; set; } = null!;

    public override string ToString()
    {
        return $"Fine Payment ID: {Id}, Fine ID: {Fineid}, Amount: {Amountpaid}, Date: {Paymentdate}";
    }
}
