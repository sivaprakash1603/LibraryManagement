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

        public (decimal fineAmount, decimal totalPaid, decimal remainingBalance, bool isPaid) GetFinePaymentSummary(int fineId)
        {
            var fine = _context.Fines
                               .AsNoTracking()
                               .FirstOrDefault(f => f.Id == fineId);

            if (fine == null)
            {
                throw new KeyNotFoundException($"No fine found with ID: {fineId}");
            }

            var totalPaid = _context.Finepayments
                                    .Where(fp => fp.Fineid == fineId)
                                    .Sum(fp => (decimal?)fp.Amountpaid) ?? 0m;

            var remainingBalance = fine.Fineamount - totalPaid;
            if (remainingBalance < 0m)
            {
                remainingBalance = 0m;
            }

            return (fine.Fineamount, totalPaid, remainingBalance, fine.Ispaid);
        }

        public Finepayment RecordFinePayment(int fineId, decimal amountPaid)
        {
            var fine = _context.Fines.Find(fineId);
            if (fine == null)
            {
                throw new KeyNotFoundException($"No fine found with ID: {fineId}");
            }

            var totalPaidBeforePayment = _context.Finepayments
                                                 .Where(fp => fp.Fineid == fineId)
                                                 .Sum(fp => (decimal?)fp.Amountpaid) ?? 0m;

            var finePayment = new Finepayment
            {
                Fineid = fineId,
                Paymentdate = DateTime.Now,
                Amountpaid = amountPaid
            };

            _context.Finepayments.Add(finePayment);

            var totalPaidAfterPayment = totalPaidBeforePayment + amountPaid;
            if (totalPaidAfterPayment >= fine.Fineamount)
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
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                try
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = "SELECT calculate_member_fine(@memberId)";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@memberId";
                    parameter.Value = memberId;
                    command.Parameters.Add(parameter);

                    var result = command.ExecuteScalar();
                    return result == null || result == DBNull.Value ? 0m : Convert.ToDecimal(result);
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
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