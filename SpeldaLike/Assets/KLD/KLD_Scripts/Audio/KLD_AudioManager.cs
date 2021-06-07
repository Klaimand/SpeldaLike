using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup group;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0f;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.outputAudioMixerGroup = group;
    }

    public AudioSource GetSource()
    {
        return source;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }

}


public class KLD_AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    [SerializeField]
    Sound[] sounds;

    [SerializeField]
    string[] soundsToPlayOnStart;

    public static KLD_AudioManager Instance = null;

    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.parent = transform;
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }

        foreach (string sound in soundsToPlayOnStart)
        {
            PlaySound(sound);
        }
        GetSound("musique1").GetSource().loop = true;
        GetSound("musique2").GetSource().loop = true;
    }

    public void PlaySound(string _name)
    {
        bool foundsmthng = false;
        foreach (Sound sound in sounds)
        {
            if (sound.name == _name)
            {
                sound.Play();
                foundsmthng = true;
                //return;
            }
        }
        if (!foundsmthng)
            Debug.LogWarning("No found sound '" + _name + "'");
    }

    public Sound GetSound(string _name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == _name)
            {
                return sound;
            }
        }

        Debug.LogError("Not able to get sound '" + _name + "'");
        return null;

    }

    public void FadeOutInst(AudioSource _source, float time)
    {
        StartCoroutine(FadeOut(_source, time));
    }

    IEnumerator FadeOut(AudioSource _source, float time)
    {
        float curTime = 0f;
        float startVolume = _source.volume;

        while (curTime < time)
        {
            _source.volume = Mathf.Lerp(startVolume, 0f, curTime / time);
            curTime += Time.deltaTime;
            yield return null;
        }
        _source.volume = 0f;

        _source.Stop();
    }

    public void SetReverb(bool value)
    {
        mixer.SetFloat("ReverbMix", value ? 0f : -80f);
    }
}