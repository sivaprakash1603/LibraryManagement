using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementDALLibrary.Contexts;
using LibraryManagementDALLibrary.Interfaces;
using LibraryManagementModelLibrary.Models;   

namespace LibraryManagementDALLibrary.Repositories
{
    public abstract class AbstractRepository<K,T> : IRepository<K,T> where K : notnull where T : class
    {
        protected LibraryContext _context;

        public AbstractRepository()
        {
            _context = new LibraryContext();
        }
        public T? Create(T item)
        {
            _context.Add(item);
            _context.SaveChanges();
            return item;
        }

        public T? Remove(K key)
        {
            var item = Get(key);
            if (item == null){
                throw new KeyNotFoundException($"No item found with key: {key}");
            }
            _context.Remove(item);
            _context.SaveChanges();
            return item;
        }
        virtual public T? Get(K key)
        {
            return _context.Set<T>().Find(key);
        }
        public  List<T>? GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public  T? Update(K key, T item)
        {
            var existingItem = Get(key);
            if (existingItem == null){
                throw new KeyNotFoundException($"No item found with key: {key}");
            }
            _context.Entry(existingItem).CurrentValues.SetValues(item);
            _context.SaveChanges();
            return existingItem;
        }

        public bool Delete(K key)
        {
            var item = Get(key);
            if (item == null){
                throw new KeyNotFoundException($"No item found with key: {key}");
            }
            _context.Remove(item);
            _context.SaveChanges();
            return true;
        }

        public List<T>? GetAll(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }
    }
}
