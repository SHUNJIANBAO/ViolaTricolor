using System;
using UnityEngine;
using System.Collections.Generic;

namespace PbFramework
{
    public interface IPoolItem
    {
        void OnSpawn();
        void OnDeSpawn();
    }
    public class PoolManager
    {
        /// <summary>
        /// 资源
        /// </summary>
        static Dictionary<string, GameObject> _poolAssetsDict = new Dictionary<string, GameObject>();
        /// <summary>
        /// 对象池
        /// </summary>
        static Dictionary<string, Stack<KeyValuePair<IPoolItem, GameObject>>> _poolDict = new Dictionary<string, Stack<KeyValuePair<IPoolItem, GameObject>>>();

        /// <summary>
        /// 将资源放入对象池中
        /// </summary>
        /// <param name="goList"></param>
        public static void Push(GameObject go)
        {
            var poolItem = go.GetComponent<IPoolItem>();
            if (poolItem != null)
            {
                if (!_poolAssetsDict.ContainsKey(go.name))
                    _poolAssetsDict.Add(go.name, go);
            }
        }

        /// <summary>
        /// 将资源放入对象池中
        /// </summary>
        /// <param name="goList"></param>
        public static void Push(List<GameObject> goList)
        {
            foreach (var go in goList)
            {
                Push(go);
            }
        }

        /// <summary>
        /// 预加载指定数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Count"></param>
        public static void Preload<T>(string assetName, int Count) where T : IPoolItem
        {
            int overflowCount = Count;
            if (!_poolDict.TryGetValue(assetName, out Stack<KeyValuePair<IPoolItem, GameObject>> itemStack))
            {
                itemStack = new Stack<KeyValuePair<IPoolItem, GameObject>>();
                _poolDict.Add(assetName, itemStack);
            }
            else
            {
                overflowCount -= itemStack.Count;
            }
            if (!_poolAssetsDict.TryGetValue(assetName, out GameObject item))
            {
                throw new Exception("No Load PoolDictionary Assets!!!");
            }
            for (int i = 0; i < overflowCount; i++)
            {
                var go = GameObject.Instantiate(item);
                go.name = assetName;
                IPoolItem poolItem = go.GetComponent<IPoolItem>();
                itemStack.Push(new KeyValuePair<IPoolItem, GameObject>(poolItem, go));
                DeSpawn(go);
            }
        }

        public static bool HasPool(string assetName)
        {
            if (!_poolDict.TryGetValue(assetName, out Stack<KeyValuePair<IPoolItem, GameObject>> itemStack))
            {
                if (!_poolAssetsDict.TryGetValue(assetName, out GameObject item))
                {
                    return false;
                }
                throw new Exception("PoolDict don't Contains:" + assetName);
            }
            return true;
        }

        /// <summary>
        /// 从对象池加载对象
        /// </summary>
        /// <returns></returns>
        public static GameObject Spawn(string assetName)
        {
            IPoolItem poolItem;
            if (!_poolDict.TryGetValue(assetName, out Stack<KeyValuePair<IPoolItem, GameObject>> itemStack))
            {
                if (!_poolAssetsDict.TryGetValue(assetName, out GameObject item))
                {
                    throw new Exception("No Load PoolDictionary Assets!!!");
                }
                itemStack = new Stack<KeyValuePair<IPoolItem, GameObject>>();
                _poolDict.Add(assetName, itemStack);
                var go = GameObject.Instantiate(item);
                go.name = assetName;
                poolItem = go.GetComponent<IPoolItem>();
                itemStack.Push(new KeyValuePair<IPoolItem, GameObject>(poolItem, go));
            }
            if (itemStack.Count == 0)
            {
                var go = GameObject.Instantiate(_poolAssetsDict[assetName]);
                go.name = assetName;
                poolItem = go.GetComponent<IPoolItem>();
                poolItem.OnSpawn();
                return go;
            }
            var result = itemStack.Pop();
            poolItem = result.Key;
            result.Value.SetActive(true);
            poolItem.OnSpawn();
            return result.Value;
        }

        /// <summary>
        /// 从对象池加载对象
        /// </summary>
        /// <returns></returns>
        public static GameObject Spawn(GameObject asset)
        {
            if (!_poolAssetsDict.TryGetValue(asset.name, out GameObject item))
            {
                _poolAssetsDict.Add(asset.name, asset);
            }
            return Spawn(asset.name);
        }

        /// <summary>
        /// 将对象存进对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolItem"></param>
        public static void DeSpawn(GameObject go)
        {
            var poolItem = go.GetComponent<IPoolItem>();
            if (poolItem == null)
            {
                Debug.LogError(go.name + "  不继承自IPoolItem,无法加入对象池");
                return;
            }
            if (!_poolAssetsDict.TryGetValue(go.name, out GameObject item))
            {
                _poolAssetsDict.Add(go.name, go);
            }
            if (!_poolDict.TryGetValue(go.name, out Stack<KeyValuePair<IPoolItem, GameObject>> itemStack))
            {
                itemStack = new Stack<KeyValuePair<IPoolItem, GameObject>>();
                _poolDict.Add(go.name, itemStack);
            }
            poolItem.OnDeSpawn();
            go.SetActive(false);
            itemStack.Push(new KeyValuePair<IPoolItem, GameObject>(poolItem, go));
        }

        /// <summary>
        /// 清空指定对象池
        /// </summary>
        /// <param name="assetName"></param>
        public static void ClearPool(string assetName)
        {
            if (!_poolDict.TryGetValue(assetName, out Stack<KeyValuePair<IPoolItem, GameObject>> stack))
            {
                return;
            }
            while (stack.Count > 0)
            {
                GameObject.Destroy(stack.Pop().Value);
            }
            _poolDict.Remove(assetName);
        }
    }
}
