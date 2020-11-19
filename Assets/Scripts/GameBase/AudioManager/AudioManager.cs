using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private const int MAX_SOUND = 10; // 最多允许同时播放的音效
    private const float MAX_LOAD_SECONDS = 5; // 加载资源最久用时, 超时算作失败
    private const int MAX_CACHE = 10; // 最多缓存的音频资源个数
    private const string MUSIC_PATH = "Audio/Music/";
    private const string SOUND_PATH = "Audio/Sound/";

    private float musicVolume = 1;
    private float soundVolume = 1;
    private GameObject musicObject;
    private GameObject soundObject;
    private AudioSource musicAudio;
    private List<AudioSource> soundAudioList;

    private Dictionary<string, AudioClip> audioCacheDic;
    private Queue<string> musicCacheQueue;
    private Queue<string> soundCacheQueue;
    public AudioManager()
    {
        musicObject = new GameObject();
        Object.DontDestroyOnLoad(musicObject);
        soundObject = new GameObject();
        Object.DontDestroyOnLoad(soundObject);
        musicAudio = musicObject.AddComponent<AudioSource>();
        soundAudioList = new List<AudioSource>();
        MonoManager.Instance.AddUpdateListener(Update);
        audioCacheDic = new Dictionary<string, AudioClip>();
        musicCacheQueue = new Queue<string>();
        soundCacheQueue = new Queue<string>();
    }
    ~AudioManager()
    {
        MonoManager.Instance.RemoveUpdateListener(Update);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume < 0) { volume = 0; }
        if (volume > 1) { volume = 1; }
        musicVolume = volume;
        musicAudio.volume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        if (volume < 0) { volume = 0; }
        if (volume > 1) { volume = 1; }
        soundVolume = volume;
        for (int i = 0; i < soundAudioList.Count; ++i)
        {
            soundAudioList[i].volume = volume;
        }
    }

    public void PlayMusic(string name)
    {
        string resName = MUSIC_PATH + name;
        if (audioCacheDic.ContainsKey(resName))
        {
            musicAudio.clip = audioCacheDic[resName];
            musicAudio.Play();
        }
        else
        {
            // 保证连续调用两次不会重复加载
            if (!CoolDownManager.Instance.CheckAndSetCoolDown(BaseCoolDown.MusicLoad, MAX_LOAD_SECONDS))
            {
                return;
            }
            ResourceManager.Instance.LoadResourceAsync<AudioClip>(resName, (clip) =>
            {
                CacheMusic(resName, clip);
                musicAudio.clip = clip;
                musicAudio.Play();
                CoolDownManager.Instance.UnsetCoolDown(BaseCoolDown.MusicLoad);
            });
        }
    }

    public void PauseMusic()
    {
        if (musicAudio != null)
        {
            musicAudio.Pause();
        }
    }

    public void StopMusic()
    {
        if (musicAudio != null)
        {
            musicAudio.Stop();
        }
    }

    public void PlaySound(string name)
    {
        string resName = SOUND_PATH + name;
        while (soundAudioList.Count >= MAX_SOUND)
        {
            soundAudioList.RemoveAt(0);
        }
        if (audioCacheDic.ContainsKey(resName))
        {
            var audioSource = soundObject.AddComponent<AudioSource>();
            soundAudioList.Add(audioSource);
            audioSource.clip = audioCacheDic[resName];
            audioSource.Play();
        }
        else
        {
            // 保证连续调用两次不会重复加载
            if (!CoolDownManager.Instance.CheckAndSetCoolDown(BaseCoolDown.SoundLoad, MAX_LOAD_SECONDS))
            {
                return;
            }
            ResourceManager.Instance.LoadResourceAsync<AudioClip>(resName, (clip) =>
            {
                CacheSound(resName, clip);
                var audioSource = soundObject.AddComponent<AudioSource>();
                soundAudioList.Add(audioSource);
                audioSource.clip = clip;
                audioSource.Play();
                CoolDownManager.Instance.UnsetCoolDown(BaseCoolDown.SoundLoad);
            });
        }
    }

    public void StopSound(AudioSource source)
    {
        source.Stop();
    }

    private void CacheMusic(string name, AudioClip music)
    {
        while (musicCacheQueue.Count >= MAX_CACHE)
        {
            audioCacheDic.Remove(musicCacheQueue.Dequeue());
        }
        if (audioCacheDic.ContainsKey(name)) { return; }
        audioCacheDic.Add(name, music);
        musicCacheQueue.Enqueue(name);
    }

    private void CacheSound(string name, AudioClip music)
    {
        while (soundCacheQueue.Count >= MAX_CACHE)
        {
            audioCacheDic.Remove(soundCacheQueue.Dequeue());
        }
        if (audioCacheDic.ContainsKey(name)) { return; }
        audioCacheDic.Add(name, music);
        soundCacheQueue.Enqueue(name);
    }

    private void Update()
    {
        for (int i = 0; i < soundAudioList.Count; ++i)
        {
            if (!soundAudioList[i].isPlaying)
            {
                Object.Destroy(soundAudioList[i]);
                soundAudioList.RemoveAt(i);
            }
        }
    }
}
