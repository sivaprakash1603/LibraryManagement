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
                // First, execute the borrowing function through raw SQL
                _context.Database.ExecuteSql($"SELECT create_borrowing({memberId}, '{barcodeno}')");
                
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
                    _context.Database.ExecuteSql($"CALL return_book('{barcodeno}', '{returnDate:yyyy-MM-dd}')");
                }
                else
                {
                    _context.Database.ExecuteSql($"CALL return_book('{barcodeno}')");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning book: " + ex.Message);
            }
        }

        public (int activeBorrows, int returnedBorrows, decimal unpaidFine) GetMemberBorrowingSummary(int memberId)
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM get_member_borrowing_summary({memberId})";
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var activeBorrows = (int)reader[0];
                    var returnedBorrows = (int)reader[1];
                    var unpaidFine = (decimal)reader[2];
                    return (activeBorrows, returnedBorrows, unpaidFine);
                }
                return (0, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting member borrowing summary: " + ex.Message);
            }
        }
    }
}       