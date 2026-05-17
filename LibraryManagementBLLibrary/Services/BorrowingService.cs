using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Exceptions;
using LibraryManagementModelLibrary.Models;
namespace LibraryManagementBLLibrary.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly BorrowingRepository _borrowingRepository;
        private readonly MemberRepository _memberRepository;
        private readonly BookCopyRepository _bookCopyRepository;
        private readonly FineRepository _fineRepository;

        public BorrowingService()
        {
            _borrowingRepository = new BorrowingRepository();
            _memberRepository = new MemberRepository();
            _bookCopyRepository = new BookCopyRepository();
            _fineRepository = new FineRepository();
        }
        public int BorrowBook(int memberId, string barcodeNo)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                if (string.IsNullOrWhiteSpace(barcodeNo))
                {
                    throw new LibraryValidationException("Barcode is required.");
                }

                var member = _memberRepository.GetMemberById(memberId);
                if (member == null)
                {
                    throw new EntityNotFoundException("Member", memberId.ToString());
                }

                if (member.Accountstatus != AccountStatus.Active)
                {
                    throw new BusinessRuleViolationException($"Member {memberId} is not active.");
                }

                var unpaidFine = _fineRepository.GetTotalUnpaidFineUsingFunction(memberId);
                if (unpaidFine > 500m)
                {
                    throw new BusinessRuleViolationException($"Member {memberId} has unpaid fines above ₹500 limit.");
                }

                var activeBorrowings = _borrowingRepository.GetActiveBorrowingsByMemberId(memberId);
                if (member.Membershiptype == null)
                {
                    throw new BusinessRuleViolationException($"Member {memberId} has invalid membership type.");
                }

                if (activeBorrowings.Count >= member.Membershiptype.Maximumborrowings)
                {
                    throw new BusinessRuleViolationException($"Member {memberId} has reached the borrowing limit of {member.Membershiptype.Maximumborrowings} books.");
                }

                var bookCopy = _bookCopyRepository.GetByBarcodeNo(barcodeNo);
                if (bookCopy == null)
                {
                    throw new EntityNotFoundException("BookCopy", barcodeNo);
                }

                if (bookCopy.Status != BookcopyStatus.Available)
                {
                    throw new BusinessRuleViolationException($"Book copy {barcodeNo} is not available. Current status: {bookCopy.Status}");
                }

                if (activeBorrowings.Any(b => b.BarcodenoNavigation?.Isbn == bookCopy.Isbn))
                {
                    throw new BusinessRuleViolationException($"Member {memberId} already has an active borrowing for ISBN {bookCopy.Isbn}.");
                }

                return _borrowingRepository.CreateBorrowingUsingFunction(memberId, barcodeNo);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException($"BorrowBook - {ex.Message}", ex);
            }
        }
        public void ReturnBook(string barcodeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeNo))
                {
                    throw new LibraryValidationException("Barcode is required.");
                }

                var activeBorrowing = _borrowingRepository.GetActiveBorrowingByBarcode(barcodeNo)
                    ?? throw new EntityNotFoundException("ActiveBorrowing", barcodeNo);

                _borrowingRepository.ReturnBookUsingProcedure(barcodeNo);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("ReturnBook", ex);
            }
        }
        public List<Borrowing> GetActiveBorrowingsByMemberId(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _borrowingRepository.GetActiveBorrowingsByMemberId(memberId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetActiveBorrowingsByMemberId", ex);
            }
        }

        public List<Borrowing> GetOverdueBorrowings()
        {
            try
            {
                return _borrowingRepository.GetOverdueBorrowings();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetOverdueBorrowings", ex);
            }
        }
        public bool HasActiveBorrowingSameBook(int memberId, string isbn)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                if (string.IsNullOrWhiteSpace(isbn))
                {
                    throw new LibraryValidationException("ISBN is required.");
                }

                return _borrowingRepository.HasActiveBorrowingSameBook(memberId, isbn);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("HasActiveBorrowingSameBook", ex);
            }
        }
    }
        
}