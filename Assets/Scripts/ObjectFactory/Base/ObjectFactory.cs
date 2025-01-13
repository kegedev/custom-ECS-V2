using UnityEngine;

public class ObjectFactory<T>
{
    private readonly ObjectPool _pool;

    public ObjectFactory(GameObject prefab)
    {
        _pool = new ObjectPool(prefab);
    }

    protected T Create()
    {
        return _pool.Get().GetComponent<T>();
    }

    public void Recycle(GameObject obj)
    {
        _pool.Return(obj);
    }

    public void Preload(int count)
    {
        _pool.Preload(count);
    }

    public void Clear()
    {
        _pool.Clear();
    }
}
