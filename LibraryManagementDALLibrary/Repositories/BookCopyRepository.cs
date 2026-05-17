using LibraryManagementDALLibrary.Contexts;
using LibraryManagementDALLibrary.Interfaces;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagementDALLibrary.Repositories

{
    public class BookCopyRepository : AbstractRepository<string, Bookcopy>
    {
        public BookCopyRepository()
        {
            _context = new LibraryContext();
        }
        
        public Book? GetByIsbn(string isbn)
        {
            var bookCopy = _context.Bookcopies.Include(bc => bc.IsbnNavigation)
                                              .FirstOrDefault(bc => bc.Isbn == isbn);
            return bookCopy?.IsbnNavigation;
        }

        public List<Bookcopy> GetCopiesByIsbn(string isbn)
        {
            var bookCopies = _context.Bookcopies.Where(bc => bc.Isbn == isbn)
                                                .Include(bc => bc.IsbnNavigation)
                                                .ToList();
            return bookCopies;
        }

        public List<Bookcopy> GetAvailableCopiesByIsbn(string isbn)
        {
            var availableBookCopies = _context.Bookcopies.Where(bc => bc.Isbn == isbn && bc.Status == BookcopyStatus.Available)
                                                        .Include(bc => bc.IsbnNavigation)
                                                        .ToList();
            return availableBookCopies;
        }

        public List<Bookcopy> GetAvailableCopies()
        {
            var availableBookCopies = _context.Bookcopies.Where(bc => bc.Status == BookcopyStatus.Available)
                                                        .Include(bc => bc.IsbnNavigation)
                                                        .ToList();
            return availableBookCopies;
        }

        public Bookcopy? GetByBarcodeNo(string barcodeNo)
        {
            return _context.Bookcopies
                           .Include(bc => bc.IsbnNavigation)
                           .FirstOrDefault(bc => bc.Barcodeno == barcodeNo);
        }

        public bool UpdateStatus(string barcodeNo, BookcopyStatus newStatus)
        {
            var bookCopy = _context.Bookcopies.Find(barcodeNo);
            if (bookCopy == null)
            {
                throw new KeyNotFoundException($"No book copy found with barcode: {barcodeNo}");
            }
            bookCopy.Status = newStatus;
            _context.SaveChanges();
            return true;
        }

        public List<(string isbn, string title, string authorName, int availableCount)> GetAvailableBooksByCategory(int categoryId)
        {
            try
            {
                var results = new List<(string, string, string, int)>();
                var connection = _context.Database.GetDbConnection();
                try
                {
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT * FROM get_available_books_by_category({categoryId})";
                    var reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            var isbn = (string)reader[0];
                            var title = (string)reader[1];
                            var authorName = (string)reader[2];
                            var availableCount = (int)reader[3];
                            results.Add((isbn, title, authorName, availableCount));
                        }
                    }
                    finally
                    {
                        reader?.Dispose();
                    }
                    return results;
                }
                finally
                {
                    // Don't dispose connection - it's managed by DbContext
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting available books by category {categoryId}: " + ex.Message);
            }
        }
    }
}