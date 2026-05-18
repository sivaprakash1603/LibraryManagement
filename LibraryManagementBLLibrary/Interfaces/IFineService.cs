using System.Collections.Generic;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IFineService
{
	List<Fine> GetPendingFinesByMemberId(int memberId);
	decimal GetTotalUnpaidFine(int memberId);
	(decimal fineAmount, decimal totalPaid, decimal remainingBalance, bool isPaid) GetFinePaymentSummary(int fineId);
	bool AddFine(Fine fine);
	bool PayFine(int fineId, decimal amountPaid);
	List<Finepayment> GetFineHistoryByMemberId(int memberId);
}
