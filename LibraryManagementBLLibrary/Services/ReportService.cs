using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Exceptions;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Services
{
    public class ReportService : IReportService
    {
        private readonly BorrowingRepository _borrowingRepository;
        private readonly FineRepository _fineRepository;
        private readonly MemberRepository _memberRepository;
        private readonly BookRepository _bookRepository;
        private readonly BookCopyRepository _bookCopyRepository;

        public ReportService()
        {
            _borrowingRepository = new BorrowingRepository();
            _fineRepository = new FineRepository();
            _memberRepository = new MemberRepository();
            _bookRepository = new BookRepository();
            _bookCopyRepository = new BookCopyRepository();
        }

        public List<Borrowing> GetBooksCurrentlyBorrowed()
        {
            try
            {
                return _borrowingRepository.GetBooksCurrentlyBorrowed();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetBooksCurrentlyBorrowed", ex);
            }
        }

        public List<Borrowing> GetOverdueBooks()
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
                throw new ServiceOperationException("GetOverdueBooks", ex);
            }
        }

        public List<Member> GetMembersWithPendingFines()
        {
            try
            {
                return _fineRepository.GetPendingFines()
                                      .Select(fine => fine.Borrowing.Member)
                                      .Distinct()
                                      .ToList();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetMembersWithPendingFines", ex);
            }
        }

        public List<Book> GetMostBorrowedBooks(int topCount)
        {
            try
            {
                if (topCount <= 0)
                {
                    throw new LibraryValidationException("Top count must be greater than zero.");
                }

                return _borrowingRepository.GetMostBorrowedBooks(topCount);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetMostBorrowedBooks", ex);
            }
        }

        public List<Book> GetAvailableBooksByCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new LibraryValidationException("Category id must be greater than zero.");
                }

                var availableBooks = _bookCopyRepository.GetAvailableBooksByCategory(categoryId);
                var books = new List<Book>();

                foreach (var item in availableBooks)
                {
                    var book = _bookRepository.GetByIsbn(item.isbn);
                    if (book != null)
                    {
                        books.Add(book);
                    }
                }

                return books.Distinct().ToList();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAvailableBooksByCategory", ex);
            }
        }

        public List<Borrowing> GetMemberBorrowingHistory(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _borrowingRepository.GetMemberBorrowingHistory(memberId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetMemberBorrowingHistory", ex);
            }
        }
    }
}