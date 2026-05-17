using System.Collections.Generic;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IBorrowingService
{
	int BorrowBook(int memberId, string barcodeNo);
	void ReturnBook(string barcodeNo);
	List<Borrowing> GetActiveBorrowingsByMemberId(int memberId);
	List<Borrowing> GetOverdueBorrowings();
	bool HasActiveBorrowingSameBook(int memberId, string isbn);
}
