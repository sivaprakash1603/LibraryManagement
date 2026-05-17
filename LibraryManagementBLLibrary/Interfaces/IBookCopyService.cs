using System.Collections.Generic;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IBookCopyService
{
	Bookcopy? AddBookCopy(Bookcopy bookCopy);
	List<Bookcopy> GetCopiesByIsbn(string isbn);
	List<Bookcopy> GetAvailableCopiesByIsbn(string isbn);
	List<Bookcopy> GetAvailableCopies();
	bool MarkCopyAsBorrowed(string barcodeNo);
	bool MarkCopyAsAvailable(string barcodeNo);
	bool MarkCopyAsDamaged(string barcodeNo);
	bool MarkCopyAsUnavailable(string barcodeNo);
}
