using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementModelLibrary.Models;
using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Exceptions;

namespace LibraryManagementBLLibrary.Services
{
    public class BookService : IBookService
    {
        private readonly BookRepository _bookRepository;

        public BookService()
        {
            _bookRepository = new BookRepository();
        }

        public Book? AddBook(Book book)
        {
            try
            {
                if (book == null)
                {
                    throw new LibraryValidationException("Book details are required.");
                }

                if (string.IsNullOrWhiteSpace(book.Isbn) || string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Authorname))
                {
                    throw new LibraryValidationException("Book ISBN, title, and author are required.");
                }

                return _bookRepository.Create(book);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("AddBook", ex);
            }
        }

        public List<Book>? GetAllBooks()
        {
            try
            {
                return _bookRepository.GetAll();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAllBooks", ex);
            }
        }

        public Book? GetBookByIsbn(string isbn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    throw new LibraryValidationException("ISBN is required.");
                }

                return _bookRepository.GetByIsbn(isbn);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetBookByIsbn", ex);
            }
        }

        public Book? GetBookByTitle(string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new LibraryValidationException("Title is required.");
                }

                return _bookRepository.GetByTitle(title);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetBookByTitle", ex);
            }
        }

        public List<Book> GetBooksByAuthor(string authorName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorName))
                {
                    throw new LibraryValidationException("Author name is required.");
                }

                return _bookRepository.GetBooksByAuthor(authorName);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetBooksByAuthor", ex);
            }
        }

        public List<Book> GetBooksByCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new LibraryValidationException("Category id must be greater than zero.");
                }

                return _bookRepository.GetByCategory(categoryId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetBooksByCategory", ex);
            }
        }

        public List<Book> GetAvailableBooks()
        {
            try
            {
                return _bookRepository.GetAvailableBooks();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAvailableBooks", ex);
            }
        }

    } 
}