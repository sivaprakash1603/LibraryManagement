
namespace LibraryManagementDALLibrary.Interfaces
{
    public interface IRepository<K,T> where T : class
    {
        public T? Create(T item);
        public List<T>? GetAll();
        public T? Get(K key);
        public T? Update(K key, T item);
        public bool Delete(K key);
    }
}