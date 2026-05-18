using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Exceptions;
using LibraryManagementModelLibrary.Models;
using LibraryManagementBLLibrary.Interfaces;

namespace LibraryManagementBLLibrary.Services
{
    public class FineService : IFineService
    {
        private readonly FineRepository _fineRepository;

        public FineService()
        {
            _fineRepository = new FineRepository();
        }

        public List<Fine> GetPendingFinesByMemberId(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _fineRepository.GetPendingFinesByMemberId(memberId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetPendingFinesByMemberId", ex);
            }
        }
        public decimal GetTotalUnpaidFine(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _fineRepository.GetTotalUnpaidFineUsingFunction(memberId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetTotalUnpaidFine", ex);
            }
        }

        public (decimal fineAmount, decimal totalPaid, decimal remainingBalance, bool isPaid) GetFinePaymentSummary(int fineId)
        {
            try
            {
                if (fineId <= 0)
                {
                    throw new LibraryValidationException("Fine id must be greater than zero.");
                }

                return _fineRepository.GetFinePaymentSummary(fineId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetFinePaymentSummary", ex);
            }
        }
        public bool AddFine(Fine fine)
        {
            try
            {
                if (fine == null)
                {
                    throw new LibraryValidationException("Fine details are required.");
                }

                return _fineRepository.Create(fine) != null;
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("AddFine", ex);
            }
        }

        public bool PayFine(int fineId, decimal amountPaid)
        {
            try
            {
                if (fineId <= 0)
                {
                    throw new LibraryValidationException("Fine id must be greater than zero.");
                }

                if (amountPaid <= 0)
                {
                    throw new LibraryValidationException("Paid amount must be greater than zero.");
                }

                var summary = _fineRepository.GetFinePaymentSummary(fineId);
                if (amountPaid > summary.remainingBalance)
                {
                    throw new BusinessRuleViolationException($"Paid amount cannot be greater than the remaining balance of ₹{summary.remainingBalance:F2}.");
                }

                _fineRepository.RecordFinePayment(fineId, amountPaid);
                return true;
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("PayFine", ex);
            }
        }

        public List<Finepayment> GetFineHistoryByMemberId(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _fineRepository.GetFineHistoryByMemberId(memberId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetFineHistoryByMemberId", ex);
            }
        }

    }
}