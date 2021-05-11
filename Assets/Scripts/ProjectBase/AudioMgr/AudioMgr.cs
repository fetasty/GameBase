using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : Singleton<AudioMgr>
{
    private const int MAX_CACHE = 20;   // 最多缓存的clip资源数量
    private LinkedList<string> cacheList;     // clip资源缓存
    private Dictionary<string, AudioClip> cacheDic;
    private AudioSource backSource;     // 背景音乐音源
    private AudioSource effectSource;   // 特效音效音源
    private float backVolume;           // 背景音乐音量
    private float effectVolume;         // 特效音效音量
    private bool isMute;                // 是否静音
    private bool isResLoading;
    public AudioMgr()
    {
        cacheList = new LinkedList<string>();
        cacheDic = new Dictionary<string, AudioClip>();
        GameObject obj = new GameObject(nameof(AudioMgr));
        Object.DontDestroyOnLoad(obj);
        backSource = obj.AddComponent<AudioSource>();
        effectSource = obj.AddComponent<AudioSource>();
        backVolume = 1f;
        effectVolume = 1f;
        isMute = false;
        isResLoading = false;
    }
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="resName">资源名, 相对于Resources下的资源路径</param>
    public void PlayBackMusic(string resName)
    {
        if (cacheDic.ContainsKey(resName))
        {
            UpdateCache(resName);
            AudioClip clip = cacheDic[resName];
            if (backSource.isPlaying && backSource.clip == clip) { return; }
            backSource.clip = clip;
            backSource.Play();
            return;
        }
        else
        {
            lock(this)
            {
                if (isResLoading)
                {
                    Debug.LogWarning($"Try to play clip {resName} witch is loading");
                    return;
                }
                isResLoading = true;
            }
            ResMgr.Instance.LoadAsync<AudioClip>(resName, (clip) =>
            {
                lock(this) { isResLoading = false; }
                if (clip == null) { Debug.LogWarning($"Load clip {resName} failed!"); }
                else
                {
                    backSource.clip = clip;
                    backSource.Play();
                    UpdateCache(resName, clip);
                }
            });
        }
    }
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBackMusic()
    {
        backSource.Pause();
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBackMusic()
    {
        backSource.Stop();
    }
    /// <summary>
    /// 播放简短的特效音效
    /// </summary>
    /// <param name="resName">资源名, 相对于Resources目录的路径</param>
    public void PlayEffectAudio(string resName)
    {
        if (cacheDic.ContainsKey(resName))
        {
            UpdateCache(resName);
            AudioClip clip = cacheDic[resName];
            effectSource.PlayOneShot(clip);
            return;
        }
        else
        {
            lock (this)
            {
                if (isResLoading)
                {
                    Debug.LogWarning($"Try to play clip {resName} witch is loading");
                    return;
                }
                isResLoading = true;
            }
            ResMgr.Instance.LoadAsync<AudioClip>(resName, (clip) =>
            {
                lock (this) { isResLoading = false; }
                if (clip == null) { Debug.LogWarning($"Load clip {resName} failed!"); }
                else
                {
                    effectSource.PlayOneShot(clip);
                    UpdateCache(resName, clip);
                }
            });
        }
    }
    /// <summary>
    /// 暂停特效音效
    /// </summary>
    public void PauseEffectAudio()
    {
        effectSource.Pause();
    }
    /// <summary>
    /// 停止特效音效
    /// </summary>
    public void StopEffectAudio()
    {
        effectSource.Stop();
    }
    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="volume">音量值(0-1)</param>
    public void SetBackVolume(float volume)
    {
        backVolume = Mathf.Clamp01(volume);
        backSource.volume = backVolume;
    }
    /// <summary>
    /// 设置特效音效音量
    /// </summary>
    /// <param name="volume">音量大小(0-1)</param>
    public void SetEffectVolume(float volume)
    {
        effectVolume = Mathf.Clamp01(volume);
        effectSource.volume = effectVolume;
    }
    /// <summary>
    /// 设置静音
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SetMute(bool mute)
    {
        isMute = mute;
        backSource.mute = isMute;
        effectSource.mute = isMute;
    }
    private void UpdateCache(string resName, AudioClip clip = null)
    {
        if (cacheDic.ContainsKey(resName))
        {
            cacheList.Remove(resName);
            cacheList.AddLast(resName);
        }
        else if (clip != null)
        {
            cacheDic[resName] = clip;
            cacheList.AddLast(resName);
            if (cacheList.Count > MAX_CACHE)
            {
                string first = cacheList.First.Value;
                cacheDic.Remove(first);
                cacheList.RemoveFirst();
            }
        }
    }
}
