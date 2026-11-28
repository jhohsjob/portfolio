using System.Collections.Generic;
using UnityEngine;


public interface ISoundService
{
    void Initialize(SoundRoot soundRoot);
    void SetupDefinition(Dictionary<int, SoundDefinition> soundDatas);
    void PlayBGM(int id);
    void PlaySFX(int id);

    void StopBGM();

    void SetBGMVolume(float volume);
    void SetSFXVolume(float volume);

    void SetMute(bool mute);
}

public class SoundService : ISoundService
{
    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private Dictionary<int, SoundDefinition> _sounds = new();

    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;
    private bool _isMute;

    public SoundService()
    {
    }

    public void Initialize(SoundRoot soundRoot)
    {
        _bgmSource = soundRoot.bgmSource;
        _sfxSource = soundRoot.sfxSource;
    }

    public void SetupDefinition(Dictionary<int, SoundDefinition> soundDatas)
    {
        _sounds = new Dictionary<int, SoundDefinition>(soundDatas);
    }

    public void PlayBGM(int id)
    {
        if (_sounds.TryGetValue(id, out var sound) == false)
        {
            return;
        }

        if (sound.type != SoundType.BGM)
        {
            return;
        }

        if (_bgmSource.clip == sound.clip && _bgmSource.isPlaying)
        {
            return;
        }

        _bgmSource.clip = sound.clip;
        _bgmSource.loop = sound.loop;
        _bgmSource.volume = sound.volume * _bgmVolume;
        _bgmSource.Play();
    }

    public void PlaySFX(int id)
    {
        if (_sounds.TryGetValue(id, out var sound) == false)
        {
            return;
        }

        if (sound.type != SoundType.SFX)
        {
            return;
        }

        _sfxSource.PlayOneShot(sound.clip, sound.volume * _sfxVolume);
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        _bgmSource.volume = _bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
    }

    public void SetMute(bool mute)
    {
        _isMute = mute;

        _bgmSource.mute = mute;
        _sfxSource.mute = mute;
    }
}