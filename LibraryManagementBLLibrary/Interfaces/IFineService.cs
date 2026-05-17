using System.Collections.Generic;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IFineService
{
	List<Fine> GetPendingFinesByMemberId(int memberId);
	decimal GetTotalUnpaidFine(int memberId);
	bool AddFine(Fine fine);
	bool PayFine(int fineId, decimal amountPaid);
	List<Finepayment> GetFineHistoryByMemberId(int memberId);
}
