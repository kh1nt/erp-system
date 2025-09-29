using System.Collections.Generic;

namespace erp_system.repositories
{
    public interface IRepository<TModel, TKey>
    {
        TKey Add(TModel entity);
        void Update(TModel entity);
        void Delete(TKey id);
        TModel? GetById(TKey id);
        IEnumerable<TModel> GetAll();
    }
}


