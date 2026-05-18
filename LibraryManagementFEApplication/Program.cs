using System.Reflection.Metadata;
using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementBLLibrary.Services;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Exceptions;
using LibraryManagementModelLibrary.Models;

public class Program
{
    private readonly IBookCopyService _bookCopyService;
    private readonly IBookService _bookService;
    private readonly IBorrowingService _borrowingService;
    private readonly IReportService _reportService;
    private readonly IMemberService _memberService;
    private readonly IFineService _fineService;
    private readonly IBookCategoryService _categoryService;
    private int? _currentMemberId;

    public Program()
    {
        _bookCopyService = new BookCopyService();
        _bookService = new BookService();
        _borrowingService = new BorrowingService();
        _reportService = new ReportService();
        _memberService = new MemberService();
        _fineService = new FineService();
        _categoryService = new BookCategoryService();
        _currentMemberId = null;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine("Welcome to the Library Management System");
            Console.WriteLine("===================================================================");
            Console.WriteLine("1. Admin Login");
            Console.WriteLine("2. Member Login / Sign Up");
            Console.WriteLine("3. Exit");

            var option = ReadInt("Select an option: ");

            try
            {
                switch (option)
                {
                    case 1:
                        HandleAdminActions();
                        break;
                    case 2:
                        HandleMemberActions();
                        break;
                    case 3:
                        Console.WriteLine("Exiting application.");
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (LibraryManagementException ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
                if (!string.IsNullOrWhiteSpace(ex.ErrorCode))
                {
                    Console.WriteLine($"Error Code: {ex.ErrorCode}");
                }
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Details: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
            }
        }
    }

    private void HandleAdminActions()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine("Admin Actions");
            Console.WriteLine("===================================================================");
            Console.WriteLine("1. Member Management");
            Console.WriteLine("2. Book Management");
            Console.WriteLine("3. Fine Management");
            Console.WriteLine("4. Reports");
            Console.WriteLine("5. Borrowing History");
            Console.WriteLine("6. Back to Main Menu");

            var option = ReadInt("Please select an option: ");

            switch (option)
            {
                case 1:
                    HandleMemberManagement();
                    break;
                case 2:
                    HandleBookManagement();
                    break;
                case 3:
                    HandleAdminFineManagement();
                    break;
                case 4:
                    HandleReports();
                    break;
                case 5:
                    HandleAdminBorrowingHistory();
                    break;
                case 6:
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void HandleMemberActions()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine("Member Actions");
            Console.WriteLine("===================================================================");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Sign Up");
            Console.WriteLine("3. Back to Main Menu");

            var option = ReadInt("Please select an option: ");

            switch (option)
            {
                case 1:
                    HandleMemberLogin();
                    break;
                case 2:
                    HandleMemberSignup();
                    break;
                case 3:
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void HandleMemberDashboard()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine($"Member Dashboard (Member ID: {_currentMemberId})");
            Console.WriteLine($"Welcome to the Library Management System, {_memberService.GetMemberById(_currentMemberId.Value)?.Name}!");
            Console.WriteLine("===================================================================");
            Console.WriteLine("1. Search Book By Title");
            Console.WriteLine("2. Search Books By Author");
            Console.WriteLine("3. Search Books By Category");
            Console.WriteLine("4. View Available Books");
            Console.WriteLine("5. View My Borrowing History");
            Console.WriteLine("6. Borrow Book");
            Console.WriteLine("7. Return Book");
            Console.WriteLine("8. Fine Management");
            Console.WriteLine("10. Logout");

            var option = ReadInt("Please select an option: ");

            switch (option)
            {
                case 1:
                    HandleSearchBookByTitle();
                    break;
                case 2:
                    HandleSearchBooksByAuthor();
                    break;
                case 3:
                    HandleSearchBooksByCategory();
                    break;
                case 4:
                    HandleViewAvailableBooks();
                    break;
                case 5:
                    HandleMemberBorrowingHistory();
                    break;
                case 6:
                    HandleBorrowBook();
                    break;
                case 7:
                    HandleReturnBook();
                    break;
                case 8:
                    HandleMemberFineManagement();
                    break;
                case 10:
                    _currentMemberId = null;
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void HandleMemberBorrowingHistory()
    {
        var history = _reportService.GetMemberBorrowingHistory(_currentMemberId.Value);
        if (history.Count == 0)
        {
            Console.WriteLine("No borrowing history found.");
            return;
        }

        foreach (var borrowing in history)
        {
            Console.WriteLine(borrowing.ToString());
        }
    }

    private void HandleMemberLogin()
    {
        var memberId = ReadInt("Enter member id: ");
        var member = _memberService.GetMemberById(memberId);
        if (member == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }

        _currentMemberId = member.Id;
        Console.WriteLine($"Welcome back, {member.Name}.");
        HandleMemberDashboard();
    }

    private void HandleMemberSignup()
    {
        Console.WriteLine("\nAvailable Membership Types: 1. Basic  2. Student  3. Premium");
        var membershipTypeOption = ReadInt("Select membership type: ");
        var membershipTypeId = membershipTypeOption switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            _ => throw new InvalidOperationException("Invalid membership type option.")
        };

        var newMember = new Member
        {
            Name = ReadRequired("Enter member name: "),
            Phone = ReadRequired("Enter phone: "),
            Email = ReadRequired("Enter email: "),
            Membershiptypeid = membershipTypeId,
            Accountstatus = AccountStatus.Active
        };

        var added = _memberService.AddMember(newMember);
        _currentMemberId = added.Id;
        Console.WriteLine($"Added: {added}");
        HandleMemberDashboard();
    }

    private void HandleMemberManagement()
    {
        Console.WriteLine();
        Console.WriteLine("===================================================================");
        Console.WriteLine("Member Management");
        Console.WriteLine("===================================================================");
        Console.WriteLine("1. Add Member");
        Console.WriteLine("2. View All Members");
        Console.WriteLine("3. Search Member By Phone");
        Console.WriteLine("4. Search Member By Email");
        Console.WriteLine("5. Update Membership Status");
        Console.WriteLine("6. Deactivate Member");
        Console.WriteLine("7. Back to Main Menu");

        var option = ReadInt("Select an option: ");

        switch (option)
        {
            case 1:
                Console.WriteLine("\nAvailable Membership Types: 1. Basic  2. Student  3. Premium");
                var membershipTypeOption = ReadInt("Select membership type: ");
                var membershipTypeId = membershipTypeOption switch
                {
                    1 => 1,
                    2 => 2,
                    3 => 3,
                    _ => throw new InvalidOperationException("Invalid membership type option.")
                };
                var newMember = new Member
                {
                    Name = ReadRequired("Enter member name: "),
                    Phone = ReadRequired("Enter phone: "),
                    Email = ReadRequired("Enter email: "),
                    Membershiptypeid = membershipTypeId,
                    Accountstatus = AccountStatus.Active
                };
                var added = _memberService.AddMember(newMember);
                Console.WriteLine($"Added: {added}");
                break;
            case 2:
                PrintList(_memberService.GetAllMembers(), "No members found.");
                break;
            case 3:
                var phone = ReadRequired("Enter phone: ");
                Console.WriteLine(_memberService.GetMemberByPhone(phone)?.ToString() ?? "Member not found.");
                break;
            case 4:
                var email = ReadRequired("Enter email: ");
                Console.WriteLine(_memberService.GetMemberByEmail(email)?.ToString() ?? "Member not found.");
                break;
            case 5:
                var memberId = ReadInt("Enter member id: ");
                Console.WriteLine("1. Active  2. Inactive  3. Deactivated");
                var statusOption = ReadInt("Select status: ");
                var status = statusOption switch
                {
                    1 => AccountStatus.Active,
                    2 => AccountStatus.Inactive,
                    3 => AccountStatus.Deactivated,
                    _ => throw new InvalidOperationException("Invalid status option.")
                };
                _memberService.UpdateMembershipStatus(memberId, status);
                Console.WriteLine("Membership status updated.");
                break;
            case 6:
                _memberService.DeactivateMember(ReadInt("Enter member id: "));
                Console.WriteLine("Member deactivated.");
                break;
            case 7:
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private void HandleBookManagement()
    {
        Console.WriteLine();
        Console.WriteLine("===================================================================");
        Console.WriteLine("Book Management");
        Console.WriteLine("===================================================================");
        Console.WriteLine("1. Add Book");
        Console.WriteLine("2. Add Book Copy");
        Console.WriteLine("3. View All Book Copies");
        Console.WriteLine("4. View Available Book Copies");
        Console.WriteLine("5. Mark Copy As Damaged");
        Console.WriteLine("6. Mark Copy As Lost");
        Console.WriteLine("7. Mark Copy As Unavailable");
        Console.WriteLine("8. Mark Copy As Available");
        Console.WriteLine("9. Back to Admin Menu");

        var option = ReadInt("Select an option: ");

        switch (option)
        {
            case 1:
                var categoryName = ReadRequired("Enter category name (or select existing): ");
                var categoryId = GetOrCreateCategory(categoryName);
                var book = new Book
                {
                    Isbn = ReadRequired("Enter ISBN: "),
                    Title = ReadRequired("Enter title: "),
                    Authorname = ReadRequired("Enter author: "),
                    Edition = ReadOptional("Enter edition (optional): "),
                    Price = ReadDecimal("Enter price (₹): "),
                    Categoryid = categoryId
                };
                Console.WriteLine($"Added: {_bookService.AddBook(book)}");
                break;
            case 2:
                var copy = new Bookcopy
                {
                    Barcodeno = ReadRequired("Enter barcode: "),
                    Isbn = ReadRequired("Enter ISBN: "),
                    Status = BookcopyStatus.Available
                };
                Console.WriteLine($"Added: {_bookCopyService.AddBookCopy(copy)}");
                break;
            case 3:
                PrintList(_bookCopyService.GetAllCopies() ?? new List<Bookcopy>(), "No book copies found.");
                break;
            case 4:
                PrintList(_bookCopyService.GetAvailableCopies() ?? new List<Bookcopy>(), "No available book copies found.");
                break;
            case 5:
                _bookCopyService.MarkCopyAsDamaged(ReadRequired("Enter barcode: "));
                Console.WriteLine("Copy marked as damaged.");
                break;
            case 6:
                _bookCopyService.MarkCopyAsLost(ReadRequired("Enter barcode: "));
                Console.WriteLine("Copy marked as lost.");
                break;
            case 7:
                _bookCopyService.MarkCopyAsUnavailable(ReadRequired("Enter barcode: "));
                Console.WriteLine("Copy marked as unavailable.");
                break;
            case 8:
                _bookCopyService.MarkCopyAsAvailable(ReadRequired("Enter barcode: "));
                Console.WriteLine("Copy marked as available.");
                break;
            case 9:
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private void HandleSearchBookByTitle()
    {
        var allBooks = _bookService.GetAllBooks() ?? new List<Book>();
        if (allBooks.Count == 0)
        {
            Console.WriteLine("No books available.");
        }
        else
        {
            Console.WriteLine("\nAvailable Titles:");
            foreach (var book in allBooks)
            {
                Console.WriteLine($"  Title: {book.Title}, ISBN: {book.Isbn}");
            }
        }

        var bookByTitle = _bookService.GetBookByTitle(ReadRequired("Enter title: "));
        DisplayBookWithCopies(bookByTitle);
    }

    private void HandleSearchBooksByAuthor()
    {
        var allBooks = _bookService.GetAllBooks() ?? new List<Book>();
        var authors = allBooks.Select(book => book.Authorname).Distinct().OrderBy(author => author).ToList();
        if (authors.Count == 0)
        {
            Console.WriteLine("No authors available.");
        }
        else
        {
            Console.WriteLine("\nAvailable Authors:");
            foreach (var author in authors)
            {
                Console.WriteLine($"  {author}");
            }
        }

        var booksByAuthor = _bookService.GetBooksByAuthor(ReadRequired("Enter author: ")) ?? new List<Book>();
        DisplayBooksWithCopies(booksByAuthor, "No books found.");
    }

    private void HandleSearchBooksByCategory()
    {
        var categories = _categoryService.GetAllCategories();
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found.");
            return;
        }

        Console.WriteLine("\nAvailable Categories:");
        foreach (var category in categories)
        {
            Console.WriteLine($"  ID: {category.Id}, Name: {category.Name}");
        }

        var categoryId = ReadInt("\nEnter category id: ");
        var booksByCategory = _bookService.GetBooksByCategory(categoryId) ?? new List<Book>();
        DisplayBooksWithCopies(booksByCategory, "No books found in this category.");
    }

    private void HandleViewAvailableBooks()
    {
        PrintList(_bookCopyService.GetAvailableCopies() ?? new List<Bookcopy>(), "No available book copies found.");
    }

    private void DisplayBooksWithCopies(IEnumerable<Book> books, string emptyMessage)
    {
        var list = books?.ToList() ?? new List<Book>();
        if (list.Count == 0)
        {
            Console.WriteLine(emptyMessage);
            return;
        }

        foreach (var book in list)
        {
            DisplayBookWithCopies(book);
        }
    }

    private void DisplayBookWithCopies(Book? book)
    {
        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        Console.WriteLine(book.ToString());
        var copies = _bookCopyService.GetCopiesByIsbn(book.Isbn) ?? new List<Bookcopy>();
        if (copies.Count == 0)
        {
            Console.WriteLine("  No copies found.");
            return;
        }

        foreach (var copy in copies)
        {
            Console.WriteLine($"  Barcode: {copy.Barcodeno}, Status: {copy.Status}");
        }
    }

    private void HandleBorrowBook()
    {
        Console.WriteLine();
        if (_currentMemberId == null)
        {
            Console.WriteLine("Please login as a member first.");
            return;
        }

        Console.WriteLine("Available book copies:");
        PrintList(_bookCopyService.GetAvailableCopies() ?? new List<Bookcopy>(), "No available book copies.");

        var barcode = ReadRequired("Enter barcode: ");
        var borrowingId = _borrowingService.BorrowBook(_currentMemberId.Value, barcode);
        Console.WriteLine($"Borrowing created successfully. Borrowing ID: {borrowingId}");
    }

    private void HandleReturnBook()
    {
        Console.WriteLine();
        var barcode = ReadRequired("Enter barcode: ");
        _borrowingService.ReturnBook(barcode);
        Console.WriteLine("Book returned successfully.");
    }

    private void HandleMemberFineManagement()
    {
        Console.WriteLine();
        if (_currentMemberId == null)
        {
            Console.WriteLine("Please login as a member first.");
            return;
        }
        Console.WriteLine("===================================================================");
        Console.WriteLine("Fine Management");
        Console.WriteLine("===================================================================");
        Console.WriteLine("1. View Pending Fines");
        Console.WriteLine("2. View Total Unpaid Fine");
        Console.WriteLine("3. Pay Fine");
        Console.WriteLine("4. View Fine History");
        Console.WriteLine("5. Back to Main Menu");

        var option = ReadInt("Select an option: ");

        switch (option)
        {
            case 1:
                var memberFines = _fineService.GetPendingFinesByMemberId(_currentMemberId.Value);
                if (memberFines.Count == 0)
                {
                    Console.WriteLine("No pending fines.");
                }
                else
                {
                    foreach (var fine in memberFines)
                    {
                        var summary = _fineService.GetFinePaymentSummary(fine.Id);
                        Console.WriteLine($"Fine ID: {fine.Id}, Amount: ₹{summary.fineAmount:F2}, Paid: ₹{summary.totalPaid:F2}, Remaining: ₹{summary.remainingBalance:F2}, Type: {fine.Finetype}, Paid: {summary.isPaid}");
                    }
                }
                break;
            case 2:
                var total = _fineService.GetTotalUnpaidFine(_currentMemberId.Value);
                Console.WriteLine($"Total unpaid fine: ₹{total:F2}");
                break;
            case 3:
                var fineId = ReadInt("Enter fine id: ");
                var amountPaid = ReadDecimal("Enter paid amount (₹): ");
                _fineService.PayFine(fineId, amountPaid);
                var paymentSummary = _fineService.GetFinePaymentSummary(fineId);
                Console.WriteLine("Fine payment recorded.");
                Console.WriteLine($"Total paid: ₹{paymentSummary.totalPaid:F2}");
                Console.WriteLine($"Remaining balance: ₹{paymentSummary.remainingBalance:F2}");
                break;
            case 4:
                var fineHistory = _fineService.GetFineHistoryByMemberId(_currentMemberId.Value);
                if (fineHistory.Count == 0)
                {
                    Console.WriteLine("No fine history.");
                }
                else
                {
                    foreach (var payment in fineHistory)
                    {
                        Console.WriteLine($"Payment ID: {payment.Id}, Amount: ₹{payment.Amountpaid:F2}, Date: {payment.Paymentdate}");
                    }
                }
                break;
            case 5:
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private void HandleAdminFineManagement()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine("Admin Fine Management");
            Console.WriteLine("===================================================================");
            Console.WriteLine("1. View Pending Fines Of Member");
            Console.WriteLine("2. View Total Unpaid Fine Of Member");
            Console.WriteLine("3. View Fine History Of Member");
            Console.WriteLine("4. Back to Admin Menu");

            var option = ReadInt("Select an option: ");

            switch (option)
            {
                case 1:
                    ShowPendingFinesForMember(ReadInt("Enter member id: "));
                    break;
                case 2:
                    ShowTotalUnpaidFineForMember(ReadInt("Enter member id: "));
                    break;
                case 3:
                    ShowFineHistoryForMember(ReadInt("Enter member id: "));
                    break;
                case 4:
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void ShowPendingFinesForMember(int memberId)
    {
        var memberFines = _fineService.GetPendingFinesByMemberId(memberId);
        if (memberFines.Count == 0)
        {
            Console.WriteLine("No pending fines.");
            return;
        }

        foreach (var fine in memberFines)
        {
            var summary = _fineService.GetFinePaymentSummary(fine.Id);
            Console.WriteLine($"Fine ID: {fine.Id}, Amount: ₹{summary.fineAmount:F2}, Paid: ₹{summary.totalPaid:F2}, Remaining: ₹{summary.remainingBalance:F2}, Type: {fine.Finetype}, Paid: {summary.isPaid}");
        }
    }

    private void ShowTotalUnpaidFineForMember(int memberId)
    {
        var total = _fineService.GetTotalUnpaidFine(memberId);
        Console.WriteLine($"Total unpaid fine: ₹{total:F2}");
    }

    private void ShowFineHistoryForMember(int memberId)
    {
        var fineHistory = _fineService.GetFineHistoryByMemberId(memberId);
        if (fineHistory.Count == 0)
        {
            Console.WriteLine("No fine history.");
            return;
        }

        foreach (var payment in fineHistory)
        {
            Console.WriteLine($"Payment ID: {payment.Id}, Amount: ₹{payment.Amountpaid:F2}, Date: {payment.Paymentdate}");
        }
    }

    private void HandleAdminBorrowingHistory()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===================================================================");
            Console.WriteLine("Borrowing History Management");
            Console.WriteLine("===================================================================");
            Console.WriteLine("1. View Borrowing History By Member");
            Console.WriteLine("2. View All Active Borrowings");
            Console.WriteLine("3. Back to Admin Menu");

            var option = ReadInt("Please select an option: ");

            switch (option)
            {
                case 1:
                    ShowBorrowingHistoryForMember(ReadInt("Enter member id: "));
                    break;
                case 2:
                    ShowAllActiveBorrowings();
                    break;
                case 3:
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void ShowBorrowingHistoryForMember(int memberId)
    {
        var history = _reportService.GetMemberBorrowingHistory(memberId);
        if (history.Count == 0)
        {
            Console.WriteLine("No borrowing history found for this member.");
            return;
        }

        Console.WriteLine($"\nBorrowing History for Member ID {memberId}:");
        foreach (var borrowing in history)
        {
            Console.WriteLine(borrowing.ToString());
        }
    }

    private void ShowAllActiveBorrowings()
    {
        var activeBorrowings = _reportService.GetBooksCurrentlyBorrowed();
        if (activeBorrowings.Count == 0)
        {
            Console.WriteLine("No active borrowings.");
            return;
        }

        Console.WriteLine("\nAll Active Borrowings:");
        foreach (var borrowing in activeBorrowings)
        {
            Console.WriteLine(borrowing.ToString());
        }
    }

    private void HandleReports()
    {
        Console.WriteLine();
        Console.WriteLine("===================================================================");
        Console.WriteLine("Reports");
        Console.WriteLine("===================================================================");
        Console.WriteLine("1. Books Currently Borrowed");
        Console.WriteLine("2. Overdue Books");
        Console.WriteLine("3. Members With Pending Fines");
        Console.WriteLine("4. Most Borrowed Books");
        Console.WriteLine("5. Available Books By Category");
        Console.WriteLine("6. Member Borrowing History");
        Console.WriteLine("7. Back to Main Menu");

        var option = ReadInt("Select an option: ");

        switch (option)
        {
            case 1:
                PrintList(_reportService.GetBooksCurrentlyBorrowed(), "No active borrowings.");
                break;
            case 2:
                PrintList(_reportService.GetOverdueBooks(), "No overdue books.");
                break;
            case 3:
                PrintList(_reportService.GetMembersWithPendingFines(), "No members with pending fines.");
                break;
            case 4:
                PrintList(_reportService.GetMostBorrowedBooks(ReadInt("Enter top count: ")), "No data available.");
                break;
            case 5:
                PrintList(_reportService.GetAvailableBooksByCategory(ReadInt("Enter category id: ")), "No available books in this category.");
                break;
            case 6:
                PrintList(_reportService.GetMemberBorrowingHistory(ReadInt("Enter member id: ")), "No borrowing history.");
                break;
            case 7:
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private static void PrintList<T>(IEnumerable<T> items, string emptyMessage)
    {
        var list = items?.ToList() ?? new List<T>();
        if (list.Count == 0)
        {
            Console.WriteLine(emptyMessage);
            return;
        }

        foreach (var item in list)
        {
            Console.WriteLine(item);
        }
    }

    private static int ReadInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(), out var value))
            {
                return value;
            }

            Console.WriteLine("Please enter a valid number.");
        }
    }

    private static decimal ReadDecimal(string message)
    {
        while (true)
        {
            Console.Write(message);
            if (decimal.TryParse(Console.ReadLine(), out var value))
            {
                return value;
            }

            Console.WriteLine("Please enter a valid decimal value.");
        }
    }

    private static string ReadRequired(string message)
    {
        while (true)
        {
            Console.Write(message);
            var value = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("Value is required.");
        }
    }

    private static string? ReadOptional(string message)
    {
        Console.Write(message);
        var value = Console.ReadLine()?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private int GetOrCreateCategory(string categoryName)
    {
        return _categoryService.GetOrCreateCategoryByName(categoryName);
    }

    public static void Main(string[] args)
    {
        var program = new Program();
        program.Run();
    }
}