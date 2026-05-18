using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementModelLibrary.Models;
using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Exceptions;

namespace LibraryManagementBLLibrary.Services
{
    public class BookCopyService : IBookCopyService
    {   

        private readonly BookCopyRepository _bookCopyRepository;
        private readonly BookRepository _bookRepository;
        private readonly BorrowingRepository _borrowingRepository;
        private readonly FineRepository _fineRepository;
        

        public BookCopyService()
        {
            _bookCopyRepository = new BookCopyRepository();
            _bookRepository = new BookRepository();
            _borrowingRepository = new BorrowingRepository();
            _fineRepository = new FineRepository();
        }

        public Bookcopy? AddBookCopy(Bookcopy bookCopy)
        {
            try
            {
                if (bookCopy == null)
                {
                    throw new LibraryValidationException("Book copy details are required.");
                }

                if (string.IsNullOrWhiteSpace(bookCopy.Barcodeno) || string.IsNullOrWhiteSpace(bookCopy.Isbn))
                {
                    throw new LibraryValidationException("Barcode and ISBN are required.");
                }

                var bookExists = _bookRepository.GetByIsbn(bookCopy.Isbn) != null;
                if (!bookExists)
                {
                    throw new EntityNotFoundException("Book", bookCopy.Isbn);
                }

                return _bookCopyRepository.Create(bookCopy);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("AddBookCopy", ex);
            }
        }

        public List<Bookcopy> GetAvailableCopies()
        {
            try
            {
                return _bookCopyRepository.GetAvailableCopies();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAvailableCopies", ex);
            }
        }

        public List<Bookcopy> GetCopiesByIsbn(string isbn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    throw new LibraryValidationException("ISBN is required.");
                }

                return _bookCopyRepository.GetCopiesByIsbn(isbn);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetCopiesByIsbn", ex);
            }
        }

        public bool MarkCopyAsAvailable(string barcodeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeNo))
                {
                    throw new LibraryValidationException("Barcode is required.");
                }

                return _bookCopyRepository.UpdateStatus(barcodeNo, BookcopyStatus.Available);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("MarkCopyAsAvailable", ex);
            }
        }

        public bool MarkCopyAsDamaged(string barcodeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeNo))
                {
                    throw new LibraryValidationException("Barcode is required.");
                }

                var bookCopy = GetBookCopyAndMaybeChargeFine(barcodeNo, FineType.Damage, 0.20m);
                return _bookCopyRepository.UpdateStatus(bookCopy.Barcodeno, BookcopyStatus.Damaged);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("MarkCopyAsDamaged", ex);
            }
        }

        public bool MarkCopyAsLost(string barcodeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeNo))
                {
                    throw new LibraryValidationException("Barcode is required.");
                }

                var bookCopy = GetBookCopyAndMaybeChargeFine(barcodeNo, FineType.Lost, 1.0m);
                return _bookCopyRepository.UpdateStatus(bookCopy.Barcodeno, BookcopyStatus.Lost);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("MarkCopyAsLost", ex);
            }
        }

        public bool MarkCopyAsUnavailable(string barcodeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeNo))
                {
                    throw new LibraryValidationException("Barcode is required.");
                }

                var bookCopy = GetBookCopyAndMaybeChargeFine(barcodeNo, FineType.Other, 0.20m);
                return _bookCopyRepository.UpdateStatus(bookCopy.Barcodeno, BookcopyStatus.Unavailable);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("MarkCopyAsUnavailable", ex);
            }
        }

        private Bookcopy GetBookCopyAndMaybeChargeFine(string barcodeNo, FineType fineType, decimal fineRate)
        {
            var bookCopy = _bookCopyRepository.GetByBarcodeNo(barcodeNo)
                           ?? throw new EntityNotFoundException("BookCopy", barcodeNo);

            var activeBorrowing = _borrowingRepository.GetActiveBorrowingByBarcode(barcodeNo);
            if (activeBorrowing != null && bookCopy.IsbnNavigation != null)
            {
                var bookPrice = bookCopy.IsbnNavigation.Price;
                var fineAmount = fineType == FineType.Lost
                    ? bookPrice
                    : decimal.Round(bookPrice * fineRate, 2, MidpointRounding.AwayFromZero);

                var fine = new Fine
                {
                    Borrowingid = activeBorrowing.Id,
                    Finetype = fineType,
                    Fineamount = fineAmount,
                    Ispaid = false
                };

                _fineRepository.Create(fine);
                // Auto-close the active borrowing when admin marks the copy as damaged/lost/unavailable
                // This ensures the borrowing is no longer active and the fine remains linked to that borrowing.
                try
                {
                    _borrowingRepository.ReturnBookUsingProcedure(activeBorrowing.Barcodeno, DateOnly.FromDateTime(DateTime.Today));
                }
                catch
                {
                    // If return procedure fails, we still created the fine. Bubble up as a service exception elsewhere.
                }
            }

            return bookCopy;
        }


        public List<Bookcopy> GetAllCopies()
        {
            try
            {
                var copies = _bookCopyRepository.GetAllCopiesWithBookDetails();
                return copies ?? new List<Bookcopy>();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAllCopies", ex);
            }
        }
    }
}