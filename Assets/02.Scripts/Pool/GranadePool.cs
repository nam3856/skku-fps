using System.Collections.Generic;
using UnityEngine;

public class GranadePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private int _initialPoolSize = 10;

    private Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolParent;

    private void Awake()
    {
        _poolParent = new GameObject("GrenadePool").transform;
        _poolParent.SetParent(transform);
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewGrenade();
        }
    }

    private GameObject CreateNewGrenade()
    {
        GameObject grenade = Instantiate(_grenadePrefab, _poolParent);
        grenade.SetActive(false);
        _pool.Enqueue(grenade);
        return grenade;
    }

    public GameObject GetGrenade()
    {
        if (_pool.Count == 0)
        {
            CreateNewGrenade();
        }

        GameObject grenade = _pool.Dequeue();
        grenade.SetActive(true);
        return grenade;
    }

    public void ReturnGrenade(GameObject grenade)
    {
        grenade.SetActive(false);
        grenade.transform.SetParent(_poolParent);
        _pool.Enqueue(grenade);
    }
} 