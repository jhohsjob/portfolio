using UnityEngine;


public class SoundRoot : MonoBehaviour
{
    [SerializeField]
    private AudioSource _bgmSource;
    [SerializeField]
    private AudioSource _sfxSource;

    public AudioSource bgmSource => _bgmSource;
    public AudioSource sfxSource => _sfxSource;
}