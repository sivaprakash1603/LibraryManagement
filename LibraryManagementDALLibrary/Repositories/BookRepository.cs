using LibraryManagementDALLibrary.Contexts;
using LibraryManagementDALLibrary.Interfaces;
using LibraryManagementModelLibrary.Models;
using LibraryManagementModelLibrary.Enums;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagementDALLibrary.Repositories


{
    public class BookRepository : AbstractRepository<int, Book>
    {
        public BookRepository()
        {
            _context = new LibraryContext();
        }

        public Book? GetByIsbn(string isbn)
        {
            return _context.Books.FirstOrDefault(b => b.Isbn == isbn);
        }

        public Book? GetByTitle(string title)
        {
            return _context.Books.FirstOrDefault(b => b.Title == title);
        }

        public List<Book> GetByCategory(int categoryId)
        {
            return _context.Books.Where(b => b.Categoryid == categoryId).ToList();

        }

        public List<Book> GetBooksByAuthor(string authorName)
        {
            return _context.Books.Where(b => b.Authorname == authorName).ToList();
        }

}
}