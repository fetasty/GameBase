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
    /// �첽���س���, Ҫ���صĳ�����Ҫ������ӵ�BuildSettings�ĳ����б���
    /// </summary>
    /// <param name="sceneName">��������</param>
    /// <param name="callback">�������سɹ��ص�, Ĭ��Ϊnull</param>
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
