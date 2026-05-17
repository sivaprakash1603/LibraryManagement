using System.Collections.Generic;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IBookService
{
	Book? AddBook(Book book);
	List<Book>? GetAllBooks();
	Book? GetBookByIsbn(string isbn);
	Book? GetBookByTitle(string title);
	List<Book>? GetBooksByAuthor(string authorName);
	List<Book>? GetBooksByCategory(int categoryId);
	List<Book>? GetAvailableBooks();
}
