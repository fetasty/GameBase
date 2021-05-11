using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : Singleton<AudioMgr>
{
    private const int MAX_CACHE = 20;   // ��໺���clip��Դ����
    private LinkedList<string> cacheList;     // clip��Դ����
    private Dictionary<string, AudioClip> cacheDic;
    private AudioSource backSource;     // ����������Դ
    private AudioSource effectSource;   // ��Ч��Ч��Դ
    private float backVolume;           // ������������
    private float effectVolume;         // ��Ч��Ч����
    private bool isMute;                // �Ƿ���
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
    /// ���ű�������
    /// </summary>
    /// <param name="resName">��Դ��, �����Resources�µ���Դ·��</param>
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
    /// ��ͣ��������
    /// </summary>
    public void PauseBackMusic()
    {
        backSource.Pause();
    }
    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopBackMusic()
    {
        backSource.Stop();
    }
    /// <summary>
    /// ���ż�̵���Ч��Ч
    /// </summary>
    /// <param name="resName">��Դ��, �����ResourcesĿ¼��·��</param>
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
    /// ��ͣ��Ч��Ч
    /// </summary>
    public void PauseEffectAudio()
    {
        effectSource.Pause();
    }
    /// <summary>
    /// ֹͣ��Ч��Ч
    /// </summary>
    public void StopEffectAudio()
    {
        effectSource.Stop();
    }
    /// <summary>
    /// ���ñ�����������
    /// </summary>
    /// <param name="volume">����ֵ(0-1)</param>
    public void SetBackVolume(float volume)
    {
        backVolume = Mathf.Clamp01(volume);
        backSource.volume = backVolume;
    }
    /// <summary>
    /// ������Ч��Ч����
    /// </summary>
    /// <param name="volume">������С(0-1)</param>
    public void SetEffectVolume(float volume)
    {
        effectVolume = Mathf.Clamp01(volume);
        effectSource.volume = effectVolume;
    }
    /// <summary>
    /// ���þ���
    /// </summary>
    /// <param name="mute">�Ƿ���</param>
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
