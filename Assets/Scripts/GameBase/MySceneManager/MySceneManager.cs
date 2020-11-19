using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载管理器
/// 注意, 使用SceneManager函数加载的场景一定要先在buildsettings中添加到
/// 构建的场景列表中
/// </summary>
public class MySceneManager : Singleton<MySceneManager>
{
    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="name">场景资源名</param>
    /// <param name="callback">加载完成后的回调函数</param>
    public void LoadScene(string name, UnityAction callback = null)
    {
        SceneManager.LoadScene(name);
        if (callback != null)
        {
            callback();
        }
    }

    /// <summary>
    /// 异步场景加载
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="callback">加载完成后的回调函数</param>
    public void LoadSceneAsync(string name, UnityAction callback = null)
    {
        MonoManager.Instance.StartCoroutine(
            LoadSceneCoroutine(name, callback)
        );
    }

    /// <summary>
    /// 异步场景加载的携程函数
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="callback">加载完成后的回调函数</param>
    /// <returns></returns>
    private IEnumerator LoadSceneCoroutine(string name, UnityAction callback)
    {
        var result = SceneManager.LoadSceneAsync(name);
        // 这里要循环判断是否加载完成
        while (!result.isDone)
        {
            // 通知加载进度变化
            MessageCenter.Instance.SendMessage<float>(
                BaseMessage.SceneLoadProgress,
                result.progress
            );
            yield return result.progress;
        }
        if (callback != null)
        {
            callback();
        }
    }
}
