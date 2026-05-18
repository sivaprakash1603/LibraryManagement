using LibraryManagementDALLibrary.Contexts;
using LibraryManagementModelLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementDALLibrary.Repositories
{
    public class BookCategoryRepository : AbstractRepository<int, Bookcategory>
    {
        public BookCategoryRepository()
        {
            _context = new LibraryContext();
        }

        public Bookcategory? GetByName(string name)
        {
            return _context.Bookcategories.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
        }
    }
}
