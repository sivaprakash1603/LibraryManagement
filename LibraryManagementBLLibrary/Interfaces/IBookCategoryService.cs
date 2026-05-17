using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IBookCategoryService
{
    Bookcategory AddCategory(Bookcategory category);
    Bookcategory? GetCategoryByName(string name);
    int GetOrCreateCategoryByName(string name);
    List<Bookcategory> GetAllCategories();
}
