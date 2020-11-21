using UnityEngine;

public class AudioManagerTest : MonoBehaviour
{
    public void PlayMusic()
    {
        AudioManager.Instance.PlayMusic("background");
    }

    public void PauseMusic()
    {
        AudioManager.Instance.PauseMusic();
    }

    public void StopMusic()
    {
        AudioManager.Instance.StopMusic();
    }

    public void PlaySound()
    {
        AudioManager.Instance.PlaySound("coin");
    }

}
