using System.Collections.Generic;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IReportService
{
	List<Borrowing> GetBooksCurrentlyBorrowed();
	List<Borrowing> GetOverdueBooks();
	List<Member> GetMembersWithPendingFines();
	List<Book> GetMostBorrowedBooks(int topCount);
	List<Book> GetAvailableBooksByCategory(int categoryId);
	List<Borrowing> GetMemberBorrowingHistory(int memberId);
}
