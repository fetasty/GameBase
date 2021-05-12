using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{
    public SceneMgr()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    /// <summary>
    /// 异步加载场景, 要加载的场景需要事先添加到BuildSettings的场景列表中
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="callback">场景加载成功回调, 默认为null</param>
    public void LoadSceneAsync(string sceneName, UnityAction callback = null)
    {
        MonoMgr.Instance.StartCoroutine(LoadSceneCoroutine(sceneName, callback));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, UnityAction callback)
    {
        AsyncOperation ope = SceneManager.LoadSceneAsync(sceneName);
        while(!ope.isDone)
        {
            MessageCenter.Instance.Send<float>(BaseConst.Msg_SceneLoadProgress, ope.progress);
            yield return null;
        }
        MessageCenter.Instance.Send<float>(BaseConst.Msg_SceneLoadProgress, ope.progress);
        if (callback != null) { callback.Invoke(); }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MessageCenter.Instance.Send<Scene>(BaseConst.Msg_SceneLoaded, scene);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        MessageCenter.Instance.Send<Scene>(BaseConst.Msg_SceneUnloaded, scene);
    }
}
