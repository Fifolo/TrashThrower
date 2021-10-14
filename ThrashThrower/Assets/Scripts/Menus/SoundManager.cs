using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    /*
     * MUSIC 
     */

    private AudioSource musicSource;
    private AudioSource soundSource;

    private Transform playerTransform;
    private Transform soundManagerTransform;
    protected override void Awake()
    {
        base.Awake();
        soundManagerTransform = transform;
        InstantiateAudioSources();
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    protected override void OnDestroy()
    {
        if(GameManager.Instance != null) GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
        base.OnDestroy();
    }

    private void GameManager_OnGameStateChange(GameManager.GameState from, GameManager.GameState to)
    {
        if (to == GameManager.GameState.Playing)
        {
            if (playerTransform == null) playerTransform = GameObject.Find("Player").transform;

            soundManagerTransform.SetParent(playerTransform);
        }
        else if (to == GameManager.GameState.MainMenu) soundManagerTransform.SetParent(GameManager.Instance.transform);
    }

    private void InstantiateAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        soundSource = gameObject.AddComponent<AudioSource>();
    }


    public void PlaySoundOneShot(AudioClip audioClip) => soundSource.PlayOneShot(audioClip);
    public void PlayMusicOnLoop(AudioClip audioClip) => musicSource.PlayOneShot(audioClip);
}
