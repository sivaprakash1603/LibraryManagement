using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Exceptions;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Services
{
    public class BookCategoryService : IBookCategoryService
    {
        private readonly BookCategoryRepository _categoryRepository;

        public BookCategoryService()
        {
            _categoryRepository = new BookCategoryRepository();
        }

        public Bookcategory AddCategory(Bookcategory category)
        {
            try
            {
                if (category == null)
                {
                    throw new LibraryValidationException("Category details are required.");
                }

                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    throw new LibraryValidationException("Category name is required.");
                }

                return _categoryRepository.Create(category)!;
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("AddCategory", ex);
            }
        }

        public Bookcategory? GetCategoryByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new LibraryValidationException("Category name is required.");
                }

                return _categoryRepository.GetByName(name.Trim());
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetCategoryByName", ex);
            }
        }

        public int GetOrCreateCategoryByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new LibraryValidationException("Category name is required.");
                }

                var normalizedName = name.Trim();
                var existing = GetCategoryByName(normalizedName);
                if (existing != null)
                {
                    return existing.Id;
                }

                var newCategory = new Bookcategory { Name = normalizedName };
                var created = AddCategory(newCategory);
                return created.Id;
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetOrCreateCategoryByName", ex);
            }
        }

        public List<Bookcategory> GetAllCategories()
        {
            try
            {
                return _categoryRepository.GetAll();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAllCategories", ex);
            }
        }
    }
}
