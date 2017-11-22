using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GlobalFactory {

    private static Dictionary<GameObject, Pool> m_hPools;

    static GlobalFactory() {
        m_hPools = new Dictionary<GameObject, Pool>();
    }

    public static T GetInstance<T>(GameObject hOriginal) where T : Component, Pool.IPoolable {
        return GetPool(hOriginal).Get<T>();
    }

    public static T GetInstance<T>(GameObject hOriginal, Action<T> hPostGetOps) where T : Component, Pool.IPoolable {
        T hItem = GetInstance<T>(hOriginal);
        hPostGetOps.Invoke(hItem);
        return hItem;
    }

    public static void RecycleInstance<T>(T hInstance) where T : Component, Pool.IPoolable {
        hInstance.Pool.Recycle<T>(hInstance);
    }

    public static void RecycleInstance<T>(T hInstance, Action<T> hPreRecycleOps) where T : Component, Pool.IPoolable {
        hPreRecycleOps.Invoke(hInstance);
        hInstance.Pool.Recycle<T>(hInstance);
    }

    public static Pool GetPool(GameObject hPrefab) {
        if (!m_hPools.ContainsKey(hPrefab)) {
            m_hPools.Add(hPrefab, new Pool(hPrefab));
        }
        return m_hPools[hPrefab];
    }
}

public class Pool {

    private GameObject m_hOriginal;
    private Queue<GameObject> m_hQueue;

    public Pool(GameObject hOriginal) {
        if (hOriginal.GetComponent<IPoolable>() == null) {
            throw new UnityException("Missing Component On Prefab " + hOriginal.name);
        }
        m_hOriginal = hOriginal;
        m_hQueue = new Queue<GameObject>();
    }

    public T Get<T>() where T : Component, IPoolable{
        T hItem;
        if (m_hQueue.Count == 0) {
            hItem = (GameObject.Instantiate(m_hOriginal) as GameObject).GetComponent<T>();
            hItem.Pool = this;
        }
        else {
            hItem = m_hQueue.Dequeue().GetComponent<T>();
        }
        hItem.OnGet();
        return hItem;
    }

    public T Get<T>(Action<T> hPostGetOps) where T : Component, IPoolable {
        T hItem = Get<T>();
        hPostGetOps.Invoke(hItem);
        return hItem;
    }

    public void Recycle<T>(T hItem) where T : Component, IPoolable {
        hItem.OnRecycle();
        m_hQueue.Enqueue(hItem.gameObject);
    }

    public void Recycle<T>(T hItem, Action<T> hPreRecycleOps) where T : Component, IPoolable {
        hPreRecycleOps.Invoke(hItem);
        Recycle<T>(hItem);
    }

    public interface IPoolable {

        Pool Pool { get; set; }
        void OnGet();
        void OnRecycle();

    }

}
