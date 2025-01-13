using UnityEngine;
using System.Collections.Generic;

public class ObjectPool
{
    private readonly GameObject _prefab;
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    public ObjectPool(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Object.Instantiate(_prefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }

    public void Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(_prefab);
            Return(obj);
        }
    }

    public void Clear()
    {
        while (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            Object.Destroy(obj);
        }
    }
}
