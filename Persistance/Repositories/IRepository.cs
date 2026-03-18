namespace HomeLedger.API.Persistance.Repositories;

public interface IRepository<T>
    where T : class
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T> GetByIdAsync(object id);
  void Insert(T obj);
  void Update(T obj);
  void Delete(object id);
  void Save();
}
