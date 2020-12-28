using System.Collections.Generic;

namespace BindedSources.MemoryCacheCore
{
    public interface IStoreData
    {
        void StoreData<T>(T dataToStore, string alias);
        void StoreCollection<T>(IEnumerable<T> collectionToStore, string alias);
        void ExpiratedStoreCollection<T>(IEnumerable<T> collectionToStore, string alias, int expiration = 10);
        T GetData<T>(string alias);
        IEnumerable<T> GetCollection<T>(string alias);
    }
}
