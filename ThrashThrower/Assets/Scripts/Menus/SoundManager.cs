using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private List<MusicClip> musicClips;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioSource soundSource;
    [SerializeField] private List<AudioClip> onButtonPressClips;

    private List<AudioClip> mainMenuMusic;
    private List<AudioClip> playModeMusic;

    [System.Serializable]
    private class MusicClip
    {
        public AudioClip clip;
        public MusicType musicType;
        public enum MusicType
        {
            MainMenu,
            PlayMode
        }
    }
    protected override void Awake()
    {
        base.Awake();
        mainMenuMusic = new List<AudioClip>();
        playModeMusic = new List<AudioClip>();
        ManageAudioSource();
        GameManager.Instance.OnGameStateChange += Instance_OnGameStateChange;
    }

    private void ManageAudioSource()
    {
        musicSource.loop = false;
        foreach (MusicClip musicClip in musicClips)
        {
            if (musicClip.musicType == MusicClip.MusicType.PlayMode) playModeMusic.Add(musicClip.clip);
            else mainMenuMusic.Add(musicClip.clip);
        }
    }

    private void Instance_OnGameStateChange(GameManager.GameState from, GameManager.GameState to)
    {

        if (to == GameManager.GameState.MainMenu)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }
        else if (to == GameManager.GameState.Pause)
        {
            musicSource.Pause();
        }
        else if (to == GameManager.GameState.Playing)
        {
            if (from == GameManager.GameState.Pause) musicSource.UnPause();
            else if (playModeMusic.Count > 0)
            {
                int randomTrackIndex = Random.Range(0, playModeMusic.Count);
                PlayMusic(playModeMusic[randomTrackIndex], true);
            }
        }
    }

    public void ButtonPressed()
    {
        if (onButtonPressClips.Count > 0)
        {
            int randomIndex = Random.Range(0, onButtonPressClips.Count);
            soundSource.PlayOneShot(onButtonPressClips[randomIndex]);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        musicSource.Stop();
        musicSource.loop = loop;
        musicSource.clip = clip;
        musicSource.Play();
    }
}
