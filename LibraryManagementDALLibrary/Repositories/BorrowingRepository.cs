using System.Runtime.InteropServices;
using LibraryManagementDALLibrary.Contexts;
using LibraryManagementDALLibrary.Interfaces;
using LibraryManagementModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using LibraryManagementModelLibrary.Enums;
namespace LibraryManagementDALLibrary.Repositories
{
    public class BorrowingRepository : AbstractRepository<int, Borrowing>
    {
        public BorrowingRepository()
        {
            _context = new LibraryContext();
        }

        public int CreateBorrowingUsingFunction(int memberId, string barcodeno)
        {
            try
            {
                // First, execute the borrowing function through parameterized SQL
                _context.Database.ExecuteSqlRaw(
                    "SELECT create_borrowing({0}, {1})",
                    memberId,
                    barcodeno);
                
                // Then retrieve the borrowing ID that was created
                var borrowing = _context.Borrowings
                    .Where(b => b.Barcodeno == barcodeno && b.Memberid == memberId && b.Borrowstatus == BorrowStatus.Borrowed)
                    .OrderByDescending(b => b.Id)
                    .FirstOrDefault();
                
                if (borrowing == null)
                {
                    throw new Exception("Borrowing record was not created successfully.");
                }
                
                return borrowing.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating borrowing record: " + ex.Message);
            }
        }

        public List<Borrowing> GetActiveBorrowingsByMemberId(int memberId)
        {
            return _context.Borrowings
                              .Where(b => b.Memberid == memberId && b.Borrowstatus == BorrowStatus.Borrowed)
                              .Include(b => b.BarcodenoNavigation)
                              .ThenInclude(bc => bc.IsbnNavigation)
                              .ToList();
        }

        public List<Borrowing> GetBooksCurrentlyBorrowed()
        {
            return _context.Borrowings
                           .Where(b => b.Borrowstatus == BorrowStatus.Borrowed)
                           .Include(b => b.Member)
                           .Include(b => b.BarcodenoNavigation)
                           .ThenInclude(bc => bc.IsbnNavigation)
                           .ToList();
        }

        public Borrowing? GetActiveBorrowingByBarcode(string barcodeno)
        {
            return _context.Borrowings
                           .Where(b => b.Barcodeno == barcodeno && b.Borrowstatus == BorrowStatus.Borrowed)
                           .Include(b => b.Member)
                           .Include(b => b.BarcodenoNavigation)
                           .ThenInclude(bc => bc.IsbnNavigation)
                           .OrderByDescending(b => b.Borrowdate)
                           .FirstOrDefault();
        }

        public List<Borrowing> GetOverdueBorrowings()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return _context.Borrowings
                              .Where(b => b.Duedate < today && b.Borrowstatus == BorrowStatus.Borrowed)
                              .Include(b => b.Member)
                              .Include(b => b.BarcodenoNavigation)
                              .ThenInclude(bc => bc.IsbnNavigation)
                              .ToList();
        }

        public List<Borrowing> GetMemberBorrowingHistory(int memberId)
        {
            return _context.Borrowings
                           .Where(b => b.Memberid == memberId)
                           .Include(b => b.Member)
                           .Include(b => b.BarcodenoNavigation)
                           .ThenInclude(bc => bc.IsbnNavigation)
                           .OrderByDescending(b => b.Borrowdate)
                           .ToList();
        }

        public List<Book> GetMostBorrowedBooks(int topCount)
        {
            return _context.Borrowings
                           .Include(b => b.BarcodenoNavigation)
                           .ThenInclude(bc => bc.IsbnNavigation)
                           .GroupBy(b => b.BarcodenoNavigation.Isbn)
                           .OrderByDescending(g => g.Count())
                           .Take(topCount)
                           .Select(g => g.First().BarcodenoNavigation.IsbnNavigation)
                           .Distinct()
                           .ToList();
        }
        
        public bool HasActiveBorrowingSameBook(int memberId, string isbn)
        {
            return _context.Borrowings
                              .Include(b => b.BarcodenoNavigation)
                              .Any(b => b.Memberid == memberId && b.BarcodenoNavigation.Isbn == isbn && b.Borrowstatus == BorrowStatus.Borrowed);
        }

        public void ReturnBookUsingProcedure(string barcodeno, DateOnly? returnDate = null)
        {
            try
            {
                if (returnDate.HasValue)
                {
                    _context.Database.ExecuteSqlRaw(
                        "CALL return_book({0}, {1})",
                        barcodeno,
                        returnDate.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    _context.Database.ExecuteSqlRaw(
                        "CALL return_book({0})",
                        barcodeno);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning book: " + ex.Message);
            }
        }
    }
}       