﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheCreators.PoolingSystem
{
    public class ObjectPool<T> : IPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        public ObjectPool(GameObject pooledObject, int numToSpawn = 0)
        {
            this.prefab = pooledObject;
            Spawn(numToSpawn);
        }
        private Action<T> pullObject;
        private Action<T> pushObject;
        private Stack<T> pooledObjects = new Stack<T>();
        private GameObject prefab;
        public int pooledCount
        {
            get
            {
                return pooledObjects.Count;
            }
        }
        public T Pull()
        {
            T t;
            if (pooledCount > 0)
                t = pooledObjects.Pop();
            else
                t = GameObject.Instantiate(prefab).GetComponent<T>();
            t.gameObject.SetActive(true); //ensure the object is on
            t.Initialize(Push);

            //allow default behavior and turning object back on
            pullObject?.Invoke(t);

            return t;
        }
        public GameObject PullGameObject()
        {
            return Pull().gameObject;
        }
        public GameObject PullGameObject(Vector3 position)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            return go;
        }
        public void Push(T t)
        {
            pooledObjects.Push(t);

            //create default behavior to turn off objects
            pushObject?.Invoke(t);

            t.gameObject.SetActive(false);
        }
        private void Spawn(int number)
        {
            T t;

            for (int i = 0; i < number; i++)
            {
                t = GameObject.Instantiate(prefab).GetComponent<T>();
                pooledObjects.Push(t);
                t.gameObject.SetActive(false);
            }
        }
    }
}