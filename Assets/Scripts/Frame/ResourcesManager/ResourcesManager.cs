using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class ResourcesManager
{
    //private static AssetLabelReference _label = new AssetLabelReference();
    //private static SceneInstance _oldScene;

    public static AsyncOperationHandle InstantiateAsync<T>(string assetName, Action<T> callback = null)where T:UnityEngine.Object
    {
        var handle = Addressables.InstantiateAsync(assetName);
        handle.Completed += (args) =>
        {
            callback?.Invoke(args.Result as T);
        };
        return handle;
    }
    public static AsyncOperationHandle InstantiateAsync<T>(AssetReference asset, Action<T> callback = null) where T : UnityEngine.Object
    {
        var handle = Addressables.InstantiateAsync(asset);
        handle.Completed += (args) =>
        {
            callback?.Invoke(args.Result as T);
        };
        return handle;
    }

    public static AsyncOperationHandle<T> LoadAssetAsync<T>(string assetName, Action<T> callback)
    {
        var handle = Addressables.LoadAssetAsync<T>(assetName);
        handle.Completed += (args) =>
        {
            callback?.Invoke(args.Result);
        };
        return handle;
    }

    //public static void LoadAssetsAsync<T>(string assetName, string labelName, Action<IList<T>> callback)
    //{
    //    Addressables.LoadAssetsAsync<T>(new List<object> { assetName, labelName }, null, Addressables.MergeMode.Intersection).Completed += (args) =>
    //       {
    //           callback?.Invoke(args.Result);
    //       };
    //}

    public static AsyncOperationHandle<IList<T>> LoadAssetsForLabelAsync<T>(string labelName, Action<IList<T>> callback)
    {
        //_label.labelString = labelName;
        //var handle = Addressables.LoadAssetsAsync<T>(_label, null);
        var handle = Addressables.LoadAssetsAsync<T>(labelName, null);
        handle.Completed += (args) =>
        {
            callback?.Invoke(args.Result);
        };
        return handle;
    }
    public static AsyncOperationHandle<IList<T>> LoadAssetsForLabelAsync<T>(AssetLabelReference label, Action<IList<T>> callback)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, null);
        handle.Completed += (args) =>
        {
            callback?.Invoke(args.Result);
        };
        return handle;
    }

    public static AsyncOperationHandle<IList<T>> LoadAssetsForLabelListAsync<T>(List<string> labelNameList, Action<IList<T>> callback)
    {
        IList<object> labelList = new List<object>(labelNameList);
        var handle = Addressables.LoadAssetsAsync<T>(labelList, null, Addressables.MergeMode.Intersection);
        handle.Completed += (args) =>
        {
            callback?.Invoke(args.Result);
        };
        return handle;
    }

    public static void LoadSceneAsync(string sceneName, Action<SceneInstance> callback)
    {
        Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Completed += (args) =>
        {
            callback?.Invoke(args.Result);
        };
    }
}
