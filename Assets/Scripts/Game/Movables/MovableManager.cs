using System;
using System.Collections.Generic;
using FluffyUnderware.Curvy;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovableManager : MonoBehaviour
{
    [SerializeField] public EditorReferences references = new();
    
    private Dictionary<MovableSet, List<GameObject>> cache;
    
    private void Start()
    {
        foreach (var item in references.movableSets)
        {
            item.InitializePools(transform);
        }
        
        Prespawn();
        
        if (BaseGameManager.Instance && !BaseGameManager.Instance.SpawnInfiniteMovableElements)
        {
            enabled = false;
        }
    }
    
    private void Prespawn()
    {
        foreach (var item in references.movableSets)
        {
            if (!item.IsValid) continue;
            
            switch (item.Type)
            {
                case MovableSet.SetType.People:
                {
                    foreach (var itemSpline in item.splines)
                    {
                        var randCount = Random.Range(0, 4);
                        for (var i = 0; i < randCount; i++)
                        {
                            item.PrepareNextMovable(itemSpline);
                        }
                    }
                    break;
                }
                case MovableSet.SetType.Vehicles:
                {
                    foreach (var itemSpline in item.splines)
                    {
                        item.PrepareNextMovable(itemSpline);
                    }
                    break;
                }
                case MovableSet.SetType.Animals:
                {
                    item.PrepareNextMovable();
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (!BaseGameManager.Instance) return;
        if (!BaseGameManager.Instance.HasGameStarted || BaseGameManager.Instance.IsGameStopped) return;
        
        foreach (var item in references.movableSets)
        {
            if (!item.IsValid || item.IsInCooldown) continue;
            
            if (!item.setMaxObjects)
            {
                item.PrepareNextMovable();
            }
            else
            {
                cache ??= new Dictionary<MovableSet, List<GameObject>>();
                if (!cache.ContainsKey(item))
                {
                    cache.Add(item, new List<GameObject>());
                }
                
                List<GameObject> toRemove = new();
                cache[item].ForEach(msrObject =>
                {
                    if (msrObject == null || !msrObject.activeInHierarchy)
                    {
                        toRemove.Add(msrObject);
                    }
                });
                cache[item].RemoveAll(toRemove.Contains);
                
                if (cache[item].Count < item.maxObjects)
                {
                    cache[item].Add(item.PrepareNextMovable().gameObject);
                }
            }
            
            item.RenewSpawnTime();
        }
    }
    
    [Serializable]
    public class EditorReferences
    {
        public MovableSet[] movableSets;
    }
    
    [Serializable]
    public class MovableSet
    {
        [SerializeField] string setName = "Set";
        public MovableSplineAdapter[] movablePrefabs;
        public CurvySpline[] splines;
        public float minSpawnTime = 1f;
        public float maxSpawnTime = 3f;
        public bool setMaxObjects;
        public int maxObjects = 1;
        public int initialPoolSize = 4;
        public int reallocationPoolSize = 4;
        public ObjectPool[] movablePools;
        public bool asChild;
        
        private float _spawnTime;
        
        public enum SetType
        {
            None, People, Vehicles, Animals
        }
        
        public bool IsValid => movablePrefabs is { Length: > 0 } && splines is { Length: > 0 };
        public bool IsInCooldown => Time.time - _spawnTime <= 0;
        public SetType Type
        {
            get
            {
                if (setName.Contains("People")) return SetType.People;
                if (setName.Contains("Vehicles")) return SetType.Vehicles;
                if (setName.Contains("Animals")) return SetType.Animals;
                return SetType.None;
            }
        }
        
        public void InitializePools(Transform t)
        {
            movablePools = new ObjectPool[movablePrefabs.Length];
            for (var i = 0; i < movablePrefabs.Length; i++)
            {
                var pool = new GameObject($"Pool {movablePrefabs[i].name}").AddComponent<ObjectPool>();
                if (asChild)
                {
                    pool.gameObject.transform.SetParent(t);
                }
                pool.prefab = movablePrefabs[i].gameObject;
                pool.initialSize = initialPoolSize;
                pool.reallocationSize = reallocationPoolSize;
                movablePools[i] = pool;
            }
            _spawnTime = minSpawnTime;
        }
        
        public MovableSplineAdapter PrepareNextMovable()
        {
            return PrepareNextMovable(splines[Random.Range(0, splines.Length)]);
        }
        
        public MovableSplineAdapter PrepareNextMovable(CurvySpline spline)
        {
            var msa = movablePools[Random.Range(0, movablePools.Length)].GetNext<MovableSplineAdapter>();
            msa.transform.position = ResolvePosition();
            msa.SetInitialPosition(ResolveInitialPosition());
            msa.SetSpline(spline);
            return msa;
            
            Vector3 ResolvePosition()
            {
                return new Vector3(1000, 1000, 1000);
            }
            
            float ResolveInitialPosition()
            {
                var segment = Random.Range(0, 5) * 0.2f;
                var position = Random.Range(0f, 0.2f);
                return segment + position;
            }
        }
        
        public void RenewSpawnTime()
        {
            _spawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
}
