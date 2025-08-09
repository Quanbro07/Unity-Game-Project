using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip----------")]
    public AudioClip background;
    public AudioClip levelUp;
    public AudioClip gameOver;
    public AudioClip hit;
    public AudioClip dash;
    public AudioClip boarAttack;
    public AudioClip FireSword;
    public AudioClip WaterSword;
    public AudioClip WindSword;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PauseBackground()
    {
        musicSource.Pause();
    }

    public void ResumeBackground()
    {
        musicSource.UnPause();
    }
}
