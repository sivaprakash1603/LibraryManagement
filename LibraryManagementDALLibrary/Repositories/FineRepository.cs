using Microsoft.EntityFrameworkCore;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Models;
using LibraryManagementDALLibrary.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementDALLibrary.Repositories
{
    public class FineRepository : AbstractRepository<int, Fine>
    {     

        public FineRepository()
        {
            _context = new LibraryContext();
        }
        public List<Fine> GetPendingFinesByMemberId(int memberId)
        {
            return _context.Fines
                              .Where(f => f.Borrowing.Memberid == memberId && !f.Ispaid)
                              .Include(f => f.Borrowing)
                              .ThenInclude(b => b.BarcodenoNavigation)
                              .ThenInclude(bc => bc.IsbnNavigation)
                              .ToList();
        }

        public List<Fine> GetPendingFines()
        {
            return _context.Fines
                           .Where(f => !f.Ispaid)
                           .Include(f => f.Borrowing)
                           .ThenInclude(b => b.Member)
                           .Include(f => f.Borrowing)
                           .ThenInclude(b => b.BarcodenoNavigation)
                           .ThenInclude(bc => bc.IsbnNavigation)
                           .ToList();
        }
        public decimal GetTotalPendingFinesByMemberId(int memberId)
        {
            return _context.Fines
                              .Where(f => f.Borrowing.Memberid == memberId && !f.Ispaid)
                              .Sum(f => f.Fineamount);
        }

        public void MarkFineAsPaid(int fineId)
        {
            var fine = _context.Fines.Find(fineId);
            if (fine != null)
            {
                fine.Ispaid = true;
                _context.SaveChanges();
            }
        }

        public bool AddFine(Fine fine)
        {
            _context.Fines.Add(fine);
            _context.SaveChanges();
            return true;
        }

        public List<Fine> GetAllFines()
        {
            return _context.Fines
                              .Include(f => f.Borrowing)
                              .ThenInclude(b => b.Member)
                              .Include(f => f.Borrowing)
                              .ThenInclude(b => b.BarcodenoNavigation)
                              .ThenInclude(bc => bc.IsbnNavigation)
                              .ToList();
        }

        public Finepayment RecordFinePayment(int fineId, decimal amountPaid)
        {
            var fine = _context.Fines.Find(fineId);
            if (fine == null)
            {
                throw new KeyNotFoundException($"No fine found with ID: {fineId}");
            }

            var finePayment = new Finepayment
            {
                Fineid = fineId,
                Paymentdate = DateTime.Now,
                Amountpaid = amountPaid
            };

            _context.Finepayments.Add(finePayment);

            // Update the fine's paid status if the total paid amount covers the fine amount
            var totalPaid = _context.Finepayments.Where(fp => fp.Fineid == fineId).Sum(fp => fp.Amountpaid) + amountPaid;
            if (totalPaid >= fine.Fineamount)
            {
                fine.Ispaid = true;
            }

            _context.SaveChanges();
            return finePayment;
        }

        public decimal GetTotalUnpaidFineUsingFunction(int memberId)
        {
            try
            {
                var result = _context.Database.SqlQuery<decimal>($"SELECT calculate_member_fine({memberId})").FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating total unpaid fine for member {memberId}: " + ex.Message);
            }
        }

        public List<Finepayment> GetFineHistoryByMemberId(int memberId)
        {
            return _context.Finepayments
                           .Include(fp => fp.Fine)
                           .ThenInclude(f => f.Borrowing)
                           .Where(fp => fp.Fine.Borrowing.Memberid == memberId)
                           .ToList();
        }
    }
}