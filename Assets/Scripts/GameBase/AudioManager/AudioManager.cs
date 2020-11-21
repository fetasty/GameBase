using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    private const int MAX_SOUND = 10; // 最多允许同时播放的音效
    private const float MAX_LOAD_SECONDS = 5; // 加载资源最久用时(秒), 超时算作失败; 负数表示永不超时, 一直等待
    private const int MAX_CACHE = 10; // 最多缓存的音频资源个数
    private const string MUSIC_PATH = "Audio/Music/"; // 背景音乐在Resources中的路径
    private const string SOUND_PATH = "Audio/Sound/"; // 特效音乐在Resources中的路径
    private const string MUSIC_OBJ = "MusicObject"; // 用于播放音乐的物体名称
    private const string SOUND_OBJ = "SoundObject"; // 用于播放特效音乐的物体名称

    private float musicVolume = 1; // 背景音乐音量大小, 0到1
    private float soundVolume = 1; // 特效音乐音量大小, 0到1
    private GameObject musicObject; // 用于播放音乐的物体
    private GameObject soundObject; // 用于播放音效的物体
    private AudioSource musicAudio; // 用于播放背景音乐的音源
    private List<AudioSource> soundAudioList; // 用于播放特效音乐的音源, 最多MAX_SOUND个同时播放
    private int soundAudioIndex = 0; // 下一个用于播放音效的AudioSource, MAX_SOUND个组件循环使用

    private Dictionary<string, AudioClip> audioCacheDic; // 缓存的Audio, 最多缓存MAX_CACHE个
    private Queue<string> musicCacheQueue; // 音乐clip缓存名队列
    private Queue<string> soundCacheQueue; // 音效clip缓存名队列
    public AudioManager() {
        musicObject = new GameObject(MUSIC_OBJ);
        Object.DontDestroyOnLoad(musicObject);
        soundObject = new GameObject(SOUND_OBJ);
        Object.DontDestroyOnLoad(soundObject);
        musicAudio = musicObject.AddComponent<AudioSource>();
        soundAudioList = new List<AudioSource>();
        for (int i = 0; i < MAX_SOUND; ++i) {
            soundAudioList.Add(soundObject.AddComponent<AudioSource>());
        }
        audioCacheDic = new Dictionary<string, AudioClip>();
        musicCacheQueue = new Queue<string>();
        soundCacheQueue = new Queue<string>();
    }
    /// <summary>
    /// 背景音乐音量
    /// </summary>
    public float MusicVolume {
        get { return musicVolume; }
        set {
            if (value < 0) { value = 0; }
            if (value > 1) { value = 1; }
            musicVolume = value;
            musicAudio.volume = value;
        }
    }
    /// <summary>
    /// 音效音量
    /// </summary>
    public float SoundVolume {
        get { return soundVolume; }
        set {
            if (value < 0) { value = 0; }
            if (value > 1) { value = 1; }
            soundVolume = value;
            for (int i = 0; i < soundAudioList.Count; ++i) {
                soundAudioList[i].volume = value;
            }
        }
    }
    /// <summary>
    /// 播放背景音乐, 同时只能播放一个背景音乐
    /// </summary>
    /// <param name="name">音乐资源名(可以不要后缀名), 基础路径为Resources下的MUSIC_PATH</param>
    /// <param name="loop">是否循环</param>
    public void PlayMusic(string name, bool loop = false) {
        string resName = MUSIC_PATH + name;
        if (audioCacheDic.ContainsKey(resName)) {
            musicAudio.clip = audioCacheDic[resName];
            musicAudio.loop = loop;
            musicAudio.Play();
        } else {
            // 保证连续调用两次不会重复加载
            if (!CoolDownManager.Instance.CheckAndSetCoolDown(BaseCoolDown.MusicLoad, MAX_LOAD_SECONDS)) {
                return;
            }
            ResourceManager.Instance.LoadResourceAsync<AudioClip>(resName, (clip) => {
                CacheMusic(resName, clip);
                musicAudio.clip = clip;
                musicAudio.loop = loop;
                musicAudio.Play();
                CoolDownManager.Instance.UnsetCoolDown(BaseCoolDown.MusicLoad);
            });
        }
    }
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic() {
        if (musicAudio != null) {
            musicAudio.Pause();
        }
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic() {
        if (musicAudio != null) {
            musicAudio.Stop();
        }
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效资源名(可以不要后缀名), 基础路径为Resources下的SOUND_PATH</param>
    /// <param name="loop">是否循环</param>
    public void PlaySound(string name, bool loop = false) {
        string resName = SOUND_PATH + name;
        if (audioCacheDic.ContainsKey(resName)) {
            AudioSource source = GetSoundAudioSource();
            source.clip = audioCacheDic[resName];
            source.loop = loop;
            source.Play();
        } else {
            // 保证连续调用两次不会重复加载
            if (!CoolDownManager.Instance.CheckAndSetCoolDown(BaseCoolDown.SoundLoad, MAX_LOAD_SECONDS)) {
                return;
            }
            ResourceManager.Instance.LoadResourceAsync<AudioClip>(resName, (clip) => {
                CacheSound(resName, clip);
                AudioSource source = GetSoundAudioSource();
                source.clip = clip;
                source.loop = loop;
                source.Play();
                CoolDownManager.Instance.UnsetCoolDown(BaseCoolDown.SoundLoad);
            });
        }
    }
    /// <summary>
    /// 找到一个可以使用的音效AudioSource
    /// </summary>
    /// <returns>音效AudioSource</returns>
    private AudioSource GetSoundAudioSource() {
        bool found = false;
        // 遍历一次找一个未使用的AudioSource
        for (int i = 1; i <= MAX_SOUND; ++i) {
            if (!soundAudioList[soundAudioIndex].isPlaying) {
                found = true;
                break;
            }
            soundAudioIndex = (soundAudioIndex + i) % MAX_SOUND;
        }
        if (!found) {
            // 全在使用中, 则找一个最早使用的, 非循环的AudioSource;
            // 如果全在循环中, 则返回最早使用的那个
            for (int i = 1; i <= MAX_SOUND; ++i) {
                if (!soundAudioList[soundAudioIndex].loop) {
                    break;
                }
                soundAudioIndex = (soundAudioIndex + i) % MAX_SOUND;
            }
        }
        int index = soundAudioIndex;
        soundAudioIndex = (soundAudioIndex + 1) % MAX_SOUND;
        return soundAudioList[index];
    }
    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="source">音效所在的source</param>
    public void StopSound(AudioSource source) {
        if (source != null) {
            source.Stop();
        }
    }
    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAll() {
        musicAudio.Stop();
        for (int i = 0; i < soundAudioList.Count; ++i) {
            soundAudioList[i].Stop();
        }
    }
    /// <summary>
    /// 缓存音乐的clip
    /// </summary>
    /// <param name="resName">资源路径名</param>
    /// <param name="music">音乐clip</param>
    private void CacheMusic(string resName, AudioClip music) {
        while (musicCacheQueue.Count >= MAX_CACHE) {
            audioCacheDic.Remove(musicCacheQueue.Dequeue());
        }
        if (audioCacheDic.ContainsKey(resName)) { return; }
        audioCacheDic.Add(resName, music);
        musicCacheQueue.Enqueue(resName);
    }
    /// <summary>
    /// 缓存音效clip
    /// </summary>
    /// <param name="resName">资源路径名</param>
    /// <param name="music">音效clip</param>
    private void CacheSound(string resName, AudioClip music) {
        while (soundCacheQueue.Count >= MAX_CACHE) {
            audioCacheDic.Remove(soundCacheQueue.Dequeue());
        }
        if (audioCacheDic.ContainsKey(resName)) { return; }
        audioCacheDic.Add(resName, music);
        soundCacheQueue.Enqueue(resName);
    }
}
