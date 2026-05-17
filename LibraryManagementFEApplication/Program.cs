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

    public Program()
    {
        _bookCopyService = new BookCopyService();
        _bookService = new BookService();
        _borrowingService = new BorrowingService();
        _reportService = new ReportService();
        _memberService = new MemberService();
        _fineService = new FineService();
        _categoryService = new BookCategoryService();
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Library Management System");
            Console.WriteLine("1. Member Management");
            Console.WriteLine("2. Book Management");
            Console.WriteLine("3. Borrow Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. Fine Management");
            Console.WriteLine("6. Reports");
            Console.WriteLine("7. Exit");

            var option = ReadInt("Select an option: ");

            try
            {
                switch (option)
                {
                    case 1:
                        HandleMemberManagement();
                        break;
                    case 2:
                        HandleBookManagement();
                        break;
                    case 3:
                        HandleBorrowBook();
                        break;
                    case 4:
                        HandleReturnBook();
                        break;
                    case 5:
                        HandleFineManagement();
                        break;
                    case 6:
                        HandleReports();
                        break;
                    case 7:
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

    private void HandleMemberManagement()
    {
        Console.WriteLine();
        Console.WriteLine("Member Management");
        Console.WriteLine("1. Add Member");
        Console.WriteLine("2. View All Members");
        Console.WriteLine("3. Search Member By Phone");
        Console.WriteLine("4. Search Member By Email");
        Console.WriteLine("5. Update Membership Status");
        Console.WriteLine("6. Deactivate Member");

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
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private void HandleBookManagement()
    {
        Console.WriteLine();
        Console.WriteLine("Book Management");
        Console.WriteLine("1. Add Book");
        Console.WriteLine("2. Add Book Copy");
        Console.WriteLine("3. View All Books");
        Console.WriteLine("4. View Available Books");
        Console.WriteLine("5. Search Book By Title");
        Console.WriteLine("6. Search Books By Author");
        Console.WriteLine("7. Search Books By Category");
        Console.WriteLine("8. Mark Copy As Damaged");
        Console.WriteLine("8. Mark Copy As Unavailable");

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
                PrintList(_bookService.GetAllBooks() ?? new List<Book>(), "No books found.");
                break;
            case 4:
                PrintList(_bookCopyService.GetAvailableCopies() ?? new List<Bookcopy>(), "No available book copies found.");
                break;
            case 5:
                var byTitle = _bookService.GetBookByTitle(ReadRequired("Enter title: "));
                Console.WriteLine(byTitle?.ToString() ?? "Book not found.");
                break;
            case 6:
                PrintList(_bookService.GetBooksByAuthor(ReadRequired("Enter author: ")) ?? new List<Book>(), "No books found.");
                break;
            case 7:
                PrintList(_bookService.GetBooksByCategory(ReadInt("Enter category id: ")) ?? new List<Book>(), "No books found.");
                break;
            case 8:
                _bookCopyService.MarkCopyAsDamaged(ReadRequired("Enter barcode: "));
                Console.WriteLine("Copy marked as damaged.");
                break;
            case 9:
                _bookCopyService.MarkCopyAsUnavailable(ReadRequired("Enter barcode: "));
                Console.WriteLine("Copy marked as unavailable.");
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private void HandleBorrowBook()
    {
        Console.WriteLine();
        var memberId = ReadInt("Enter member id: ");
        var barcode = ReadRequired("Enter barcode: ");
        var borrowingId = _borrowingService.BorrowBook(memberId, barcode);
        Console.WriteLine($"Borrowing created successfully. Borrowing ID: {borrowingId}");
    }

    private void HandleReturnBook()
    {
        Console.WriteLine();
        var barcode = ReadRequired("Enter barcode: ");
        _borrowingService.ReturnBook(barcode);
        Console.WriteLine("Book returned successfully.");
    }

    private void HandleFineManagement()
    {
        Console.WriteLine();
        Console.WriteLine("Fine Management");
        Console.WriteLine("1. View Pending Fines Of Member");
        Console.WriteLine("2. View Total Unpaid Fine Of Member");
        Console.WriteLine("3. Pay Fine");
        Console.WriteLine("4. View Fine History Of Member");

        var option = ReadInt("Select an option: ");

        switch (option)
        {
            case 1:
                var memberFines = _fineService.GetPendingFinesByMemberId(ReadInt("Enter member id: "));
                if (memberFines.Count == 0)
                {
                    Console.WriteLine("No pending fines.");
                }
                else
                {
                    foreach (var fine in memberFines)
                    {
                        Console.WriteLine($"Fine ID: {fine.Id}, Amount: ₹{fine.Fineamount:F2}, Type: {fine.Finetype}, Paid: {fine.Ispaid}");
                    }
                }
                break;
            case 2:
                var total = _fineService.GetTotalUnpaidFine(ReadInt("Enter member id: "));
                Console.WriteLine($"Total unpaid fine: ₹{total:F2}");
                break;
            case 3:
                _fineService.PayFine(ReadInt("Enter fine id: "), ReadDecimal("Enter paid amount (₹): "));
                Console.WriteLine("Fine payment recorded.");
                break;
            case 4:
                var fineHistory = _fineService.GetFineHistoryByMemberId(ReadInt("Enter member id: "));
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
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private void HandleReports()
    {
        Console.WriteLine();
        Console.WriteLine("Reports");
        Console.WriteLine("1. Books Currently Borrowed");
        Console.WriteLine("2. Overdue Books");
        Console.WriteLine("3. Members With Pending Fines");
        Console.WriteLine("4. Most Borrowed Books");
        Console.WriteLine("5. Available Books By Category");
        Console.WriteLine("6. Member Borrowing History");

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